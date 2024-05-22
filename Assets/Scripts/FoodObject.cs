using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UpdateScore(true);
            TriggerParticleEffect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Border"))
        {
            GameManager.Instance.UpdateScore(false);
            TriggerParticleEffect();
            Destroy(gameObject);
        }
    }

    private void TriggerParticleEffect()
    {
        if (particleSystem != null)
        {
            ParticleSystem instantiatedParticles = Instantiate(particleSystem, transform.position, Quaternion.identity);
            instantiatedParticles.Play();
            Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
        }
    }
}
