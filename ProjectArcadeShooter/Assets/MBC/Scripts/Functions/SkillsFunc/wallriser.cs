using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallriser : MonoBehaviour
{
    public Skill skill;

    MaterialPropertyBlock m_PropertyBlock;
    Renderer modelRender;

    Color baseColor;
    Texture baseTexture;

    bool colliderOpened;
    int hitNumber;

    void Start()
    {
        colliderOpened = false;

        modelRender = GetComponent<MeshRenderer>();
        m_PropertyBlock = new MaterialPropertyBlock();

        baseTexture = modelRender.sharedMaterial.GetTexture("_BaseMap");
        baseColor = Color.white;

        modelRender.material = skill.materials[1];
        m_PropertyBlock = new MaterialPropertyBlock();
        m_PropertyBlock.SetColor("_BaseColor", baseColor);
        m_PropertyBlock.SetFloat("_noiseScale", 150);
        m_PropertyBlock.SetTexture("_BaseTexture", baseTexture);
        m_PropertyBlock.SetFloat("_NoiseStrength", -2f);
        modelRender.SetPropertyBlock(m_PropertyBlock);

        StartCoroutine(disolveEffectCoroutineStart());

    }
    IEnumerator disolveEffectCoroutineStart()
    {
        m_PropertyBlock = new MaterialPropertyBlock();
        while (m_PropertyBlock.GetFloat("_NoiseStrength") < 20f)
        {
            m_PropertyBlock.SetFloat("_NoiseStrength", m_PropertyBlock.GetFloat("_NoiseStrength") + 0.5f);
            modelRender.SetPropertyBlock(m_PropertyBlock);
            yield return new WaitForSeconds(0.001f);
        }
        modelRender.material = skill.materials[0];
        colliderOpened = true;
        yield return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!colliderOpened)
        {
            return;
        }
        if(collision.gameObject.layer == 7)//ammo
        {
            hitNumber += 1;
            if (hitNumber >= 2)
            {
                StartCoroutine(disolveEffectCoroutineEnd());
            }
        }
    }
    IEnumerator disolveEffectCoroutineEnd()
    {
        modelRender.material = skill.materials[1];
        m_PropertyBlock = new MaterialPropertyBlock();
        m_PropertyBlock.SetFloat("_NoiseStrength", 20f);
        modelRender.SetPropertyBlock(m_PropertyBlock);
        while (m_PropertyBlock.GetFloat("_NoiseStrength") > -2)
        {
            m_PropertyBlock.SetFloat("_NoiseStrength", m_PropertyBlock.GetFloat("_NoiseStrength") - 0.5f);
            modelRender.SetPropertyBlock(m_PropertyBlock);
            yield return new WaitForSeconds(0.001f);
        }
        colliderOpened = false;
        Destroy(gameObject);
        yield return null;
    }

}
