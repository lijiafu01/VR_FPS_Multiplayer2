using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;

public class PlayFabCurrencyManager : MonoBehaviour
{
    private bool isLoggedIn = false; // Kiểm tra trạng thái đăng nhập
    private int coinsToAdd = 0;      // Số coin cần thêm khi đăng nhập thành công
    private int coinsToSubtract = 0; // Số coin cần trừ khi đăng nhập thành công
    private const int MaxCoinsPerDay = 300; // Số coin tối đa nhận mỗi ngày
    private bool isMaxCoinReached = false;

    // Hàm đăng nhập người dùng
    private void LoginUser(Action onLoginSuccessCallback)
    {
        Debug.Log("dev3_Attempting to login with email and password");

        string _email = PlayFabManager.Instance.UserData.Email;
        string _password = PlayFabManager.Instance.UserData.Password;

        var request = new LoginWithEmailAddressRequest
        {
            Email = _email,
            Password = _password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            OnLoginSuccess(onLoginSuccessCallback);
        }, OnLoginFailure);
    }

    private void OnLoginSuccess(Action onLoginSuccessCallback)
    {
        Debug.Log("dev3_Đăng nhập thành công!");
        isLoggedIn = true;

        // Gọi callback sau khi đăng nhập thành công
        onLoginSuccessCallback?.Invoke();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("dev3_Đăng nhập thất bại: " + error.GenerateErrorReport());
    }

    // Hàm để thêm số coin từ lớp ngoài và trả về số coin đã thêm thực tế
    public void AddGoldCoin(int coinNum, Action<int> onCoinAddedCallback)
    {
        if (isMaxCoinReached)
        {
            Debug.LogError("dev3_Không thể thêm coin, đã đạt giới hạn coin hàng ngày.");
            onCoinAddedCallback(0);  // Đã đạt giới hạn, không thêm coin
            return;
        }

        if (isLoggedIn)
        {
            CheckDailyLimit(coinNum, onCoinAddedCallback);
        }
        else
        {
            coinsToAdd = coinNum;  // Lưu số coin cần thêm
            LoginUser(() => CheckDailyLimit(coinsToAdd, onCoinAddedCallback));
        }
    }

    // Kiểm tra số coin nhận trong ngày trước khi thêm và trả về số coin thực tế đã thêm
    private void CheckDailyLimit(int coinNum, Action<int> onCoinAddedCallback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            int coinsReceivedToday = 0;
            DateTime lastReceivedDate = DateTime.MinValue;

            if (result.Data != null)
            {
                if (result.Data.ContainsKey("LastReceivedDate"))
                {
                    lastReceivedDate = DateTime.Parse(result.Data["LastReceivedDate"].Value);
                }

                if (result.Data.ContainsKey("CoinsReceivedToday"))
                {
                    coinsReceivedToday = int.Parse(result.Data["CoinsReceivedToday"].Value);
                }
            }

            DateTime currentDate = DateTime.Now;

            // Kiểm tra xem đã chuyển sang ngày mới chưa
            if (lastReceivedDate.Date != currentDate.Date)
            {
                // Reset số coin nhận trong ngày nếu là ngày mới
                coinsReceivedToday = 0;
                isMaxCoinReached = false; // Reset trạng thái giới hạn coin
            }

            // Kiểm tra nếu số coin nhận đã vượt quá giới hạn
            int coinsThatCanBeAdded = Math.Min(coinNum, MaxCoinsPerDay - coinsReceivedToday);

            if (coinsThatCanBeAdded <= 0)
            {
                Debug.LogError("dev3_Giới hạn coin hàng ngày đã đạt đến giới hạn!");
                isMaxCoinReached = true;  // Đã đạt giới hạn
                onCoinAddedCallback(0);   // Không thêm coin nào
            }
            else
            {
                // Thêm coin và cập nhật thông tin
                coinsReceivedToday += coinsThatCanBeAdded;
                AddCurrency("GC", coinsThatCanBeAdded);
                UpdateUserDailyData(currentDate, coinsReceivedToday);
                onCoinAddedCallback(coinsThatCanBeAdded); // Trả về số coin thực tế đã thêm
            }
        }, error =>
        {
            Debug.LogError("dev3_Failed to get user data: " + error.GenerateErrorReport());
            onCoinAddedCallback(0); // Nếu có lỗi, trả về 0 coin
        });
    }

    // Cập nhật số coin đã nhận trong ngày và ngày nhận gần nhất
    private void UpdateUserDailyData(DateTime currentDate, int coinsReceivedToday)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                {"LastReceivedDate", currentDate.ToString()},
                {"CoinsReceivedToday", coinsReceivedToday.ToString()}
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("dev3_Dữ liệu coin hàng ngày đã được cập nhật.");
        }, error =>
        {
            Debug.LogError("dev3_Failed to update user data: " + error.GenerateErrorReport());
        });
    }

    // Hàm để thêm tiền tệ
    public void AddCurrency(string currencyCode, int amount)
    {
        Debug.Log($"dev3_AddCurrency called, Currency: {currencyCode}, Amount: {amount}");

        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = currencyCode,
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, OnAddCurrencySuccess, OnAddCurrencyFailure);
    }

    private void OnAddCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"dev3_Tăng tiền tệ thành công! Số tiền hiện tại: {result.Balance}");
    }

    private void OnAddCurrencyFailure(PlayFabError error)
    {
        Debug.LogError($"dev3_Tăng tiền tệ thất bại: {error.GenerateErrorReport()}");
    }

   
    // Hàm để trừ số coin từ lớp ngoài và trả về số coin đã trừ thực tế
    public void SubtractGoldCoin(int coinNum, Action<int> onCoinSubtractedCallback)
    {
        if (isLoggedIn)
        {
            SubtractCurrency("GC", coinNum, onCoinSubtractedCallback);
        }
        else
        {
            coinsToSubtract = coinNum;  // Lưu số coin cần trừ
            LoginUser(() => SubtractCurrency("GC", coinsToSubtract, onCoinSubtractedCallback));
        }
    }
    // Hàm để lấy số GoldCoin hiện tại
    public void GetGoldCoinBalance(Action<int> onBalanceReceivedCallback)
    {
        if (isLoggedIn)
        {
            GetCurrencyBalance("GC", onBalanceReceivedCallback);
        }
        else
        {
            // Nếu chưa đăng nhập, đăng nhập trước rồi mới lấy số dư
            LoginUser(() => GetCurrencyBalance("GC", onBalanceReceivedCallback));
        }
    }

    // Hàm hỗ trợ để lấy số dư tiền tệ cụ thể
    private void GetCurrencyBalance(string currencyCode, Action<int> onBalanceReceivedCallback)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            int balance = 0;
            if (result.VirtualCurrency.ContainsKey(currencyCode))
            {
                balance = result.VirtualCurrency[currencyCode];
            }
            Debug.Log($"dev3_Số dư {currencyCode} hiện tại: {balance}");
            onBalanceReceivedCallback(balance);
        }, error =>
        {
            Debug.LogError($"dev3_Lỗi khi lấy số dư tiền tệ: {error.GenerateErrorReport()}");
            onBalanceReceivedCallback(0); // Trả về 0 nếu có lỗi
        });
    }
    // Hàm để trừ tiền tệ
    private void SubtractCurrency(string currencyCode, int amount, Action<int> onCurrencySubtractedCallback)
    {
        // Lấy số dư hiện tại để kiểm tra
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            int currentBalance = 0;
            if (result.VirtualCurrency.ContainsKey(currencyCode))
            {
                currentBalance = result.VirtualCurrency[currencyCode];
            }

            if (currentBalance >= amount)
            {
                var request = new SubtractUserVirtualCurrencyRequest
                {
                    VirtualCurrency = currencyCode,
                    Amount = amount
                };

                PlayFabClientAPI.SubtractUserVirtualCurrency(request, subResult =>
                {
                    Debug.Log($"dev3_Trừ tiền tệ thành công! Số tiền hiện tại: {subResult.Balance}");
                    onCurrencySubtractedCallback(subResult.Balance);
                }, error =>
                {
                    Debug.LogError($"dev3_Trừ tiền tệ thất bại: {error.GenerateErrorReport()}");
                    onCurrencySubtractedCallback(0);
                });
            }
            else
            {
                Debug.LogError("dev3_Không đủ tiền để trừ.");
                onCurrencySubtractedCallback(0);
            }
        }, error =>
        {
            Debug.LogError($"dev3_Lỗi khi lấy số dư tiền tệ: {error.GenerateErrorReport()}");
            onCurrencySubtractedCallback(0);
        });
    }
}
