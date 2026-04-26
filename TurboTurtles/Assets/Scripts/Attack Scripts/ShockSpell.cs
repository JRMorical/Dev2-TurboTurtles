using UnityEngine;
using System.Collections.Generic;

public class ShockSpell : MonoBehaviour
{
    public static void Fire(SpellStats stats, LayerMask ignoreLayer)
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit,
            stats.castDist,
            ~ignoreLayer
        );

        if (!didHit) return;

        // Spawn hit effect at primary target
        SpawnEffect(stats, hit.point);

        // Deal damage to primary target
        IDamage primaryDmg = hit.collider.GetComponentInParent<IDamage>();
        if (primaryDmg != null)
        {
            primaryDmg.takeDamage(stats.damage);
            gamemanager.instance.OnSuccessfulHit?.Invoke(hit.collider.gameObject, null);
        }

        // Chain to nearby enemies
        if (stats.chainCount <= 0) return;

        List<Collider> alreadyHit = new List<Collider> { hit.collider };
        Vector3 lastPoint = hit.point;
        int chainDamage = Mathf.RoundToInt(stats.damage * stats.chainDamageFalloff);

        for (int i = 0; i < stats.chainCount; i++)
        {
            Collider next = FindNearestEnemy(lastPoint, stats.chainRadius, alreadyHit, ignoreLayer);
            if (next == null) break;

            alreadyHit.Add(next);
            lastPoint = next.transform.position;

            SpawnEffect(stats, lastPoint);

            IDamage chainDmg = next.GetComponentInParent<IDamage>();
            if (chainDmg != null)
            {
                chainDmg.takeDamage(chainDamage);
                gamemanager.instance.OnSuccessfulHit?.Invoke(next.gameObject, null);
            }

            chainDamage = Mathf.RoundToInt(chainDamage * stats.chainDamageFalloff);
            if (chainDamage < 1) break;
        }
    }

    static Collider FindNearestEnemy(Vector3 origin, float radius, List<Collider> alreadyHit, LayerMask ignoreLayer)
    {
        Collider[] nearby = Physics.OverlapSphere(origin, radius, ~ignoreLayer);
        Collider best = null;
        float bestDist = float.MaxValue;

        foreach (Collider c in nearby)
        {
            if (alreadyHit.Contains(c)) continue;
            if (c.CompareTag("Player")) continue;
            if (c.GetComponentInParent<IDamage>() == null) continue;

            float d = Vector3.Distance(origin, c.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = c;
            }
        }

        return best;
    }

    static void SpawnEffect(SpellStats stats, Vector3 pos)
    {
        if (stats.chainEffectPrefab != null)
        {
            GameObject effect = Instantiate(stats.chainEffectPrefab, pos, Quaternion.identity);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                GameObject.Destroy(effect, ps.main.duration);
            else
                GameObject.Destroy(effect, 1f);
        }
    }
}