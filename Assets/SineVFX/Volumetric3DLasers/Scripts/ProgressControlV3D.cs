using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressControlV3D : MonoBehaviour
{
    public bool changeAllMaxLength = true;
    public float maxLength = 32f;
    public float globalProgressSpeed = 1f;
    public float globalImpactProgressSpeed = 1f;
    public bool always = true;
    public bool colorizeAll = true;
    public Color finalColor;
    [Range(0.2f, 1.0f)]
    public float gammaLinear = 1f;
    public Renderer meshRend;
    public float meshRendPower = 3f;
    public Light pointLight;
    public StartPointEffectControllerV3D startPointEffect;
    public EndPointEffectControllerV3D endPointEffect;
    public SmartWaveParticlesControllerV3D smartWaveParticles;
    public SFXControllerV3D sfxcontroller;

    private float globalProgress;
    private float globalImpactProgress;
    private LaserLineV3D[] lls;
    private LightLineV3D[] lils;
    private Renderer[] renderers;

    private void Start()
    {
        globalProgress = 0f;
        globalImpactProgress = 0f;
        lls = GetComponentsInChildren<LaserLineV3D>(true);
        lils = GetComponentsInChildren<LightLineV3D>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
  
        // Bật phát xạ tại điểm kết thúc ngay từ đầu
        if (endPointEffect != null)
        {
            endPointEffect.emit = true;
        }
    }
    public void ActivateLaser()
    {
        globalProgress = 0f;
        globalImpactProgress = 0f;
        if (endPointEffect != null)
        {
            endPointEffect.emit = true;
        }
    }

    public void DeactivateLaser()
    {
        globalProgress = 1f;
        globalImpactProgress = 1f;
        if (endPointEffect != null)
        {
            endPointEffect.emit = false;
        }
    }

    public void ChangeColor(Color color)
    {
        finalColor = color;
    }

    void Update()
    {
        // Kiểm soát Gamma và Linear modes
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

        // Gửi giá trị global progress đến các script khác
        startPointEffect.SetGlobalProgress(globalProgress);
        startPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        endPointEffect.SetGlobalProgress(globalProgress);
        endPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        smartWaveParticles.SetGlobalProgress(globalProgress);

        // Điều khiển màu sắc của tất cả các prefab con
        if (colorizeAll == true)
        {
            foreach (LightLineV3D lil in lils)
            {
                lil.SetFinalColor(finalColor);
            }
            startPointEffect.SetFinalColor(finalColor);
            endPointEffect.SetFinalColor(finalColor);
            foreach (Renderer rend in renderers)
            {
                rend.material.SetColor("_FinalColor", finalColor);
            }
        }

        // Kiểm soát tổng thể
        if (meshRend != null)
        {
            meshRend.material.SetColor("_EmissionColor", finalColor * meshRendPower);
        }

        // Giữ laser luôn hoạt động
        globalProgress = 0f;
        globalImpactProgress = 0f;
        endPointEffect.emit = true;

        // Cập nhật các giá trị progress đến các thành phần laser
        foreach (LaserLineV3D ll in lls)
        {
            ll.SetGlobalProgress(globalProgress);
            ll.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                ll.maxLength = maxLength;
            }
        }

        foreach (LightLineV3D lil in lils)
        {
            lil.SetGlobalProgress(globalProgress);
            lil.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                lil.maxLength = maxLength;
            }
        }

        sfxcontroller.SetGlobalProgress(1f - globalProgress);
    }
}
