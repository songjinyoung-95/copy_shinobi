using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{
    public enum EVfxType
    {
        Die,
        Freeze
    }

    public class VFXManager : MonoBehaviour
    {
        private const string MODEL_PATH = "VFX/VFXManager";

        private static VFXManager _instance;
        public static VFXManager  Instance
        {
            get
            {
                if(_instance == null)
                {
                    VFXManager vfx = Resources.Load<VFXManager>(MODEL_PATH);

                    if(vfx == null)
                    {
                        Debug.LogError($"{MODEL_PATH} 에 프리펩이 없습니다");
                        return null;
                    }

                    _instance = Instantiate(vfx);
                    // _instance.Init();
                }

                return _instance;
            }
        }

        [SerializeField] private VFX[] _vfxes;
        private Dictionary<EVfxType, VFX> _vfxDict;

        public void Init()
        {
            _vfxDict = new Dictionary<EVfxType, VFX>();

            foreach (var vfx in _vfxes)
            {
                if(_vfxDict.ContainsKey(vfx.VFX_TYPE))
                {
                    Debug.LogError("동일한 키를 가진 이펙트 타입이 존재합니다");
                    return;
                }

                _vfxDict.Add(vfx.VFX_TYPE, vfx);
                vfx.Generator(transform);
            }
        }

        public void ShowVFX(EVfxType vfxType, Vector3 spawn)
        {
            if(!_vfxDict.ContainsKey(vfxType))
            {
                Debug.LogError($"{vfxType} 파티클을 미리 정의 후 사용해주세요");
                return;
            }

            _vfxDict[vfxType].Spawn(spawn);
        }

        public void ShowVFX(EVfxType vfxType, Vector3 spawn, Transform parent)
        {
            if(!_vfxDict.ContainsKey(vfxType))
            {
                Debug.LogError($"{vfxType} 파티클을 미리 정의 후 사용해주세요");
                return;
            }

            _vfxDict[vfxType].Spawn(spawn,parent);
        }



        [Serializable]
        private class VFX
        {
            public EVfxType VFX_TYPE;
            public ParticleSystem Original;
            public int Count;

            private List<ParticleSystem> _particles;

            public void Generator(Transform parent)
            {
                _particles = new List<ParticleSystem>();

                for (int i = 0; i < Count; i++)
                {
                    ParticleSystem particle = Instantiate(Original, parent);
                    particle.gameObject.SetActive(false);
                    _particles.Add(particle);
                }
            }

            public void Spawn(Vector3 position)
            {
                for (int i = 0; i < _particles.Count; i++)
                {
                    ParticleSystem particle = _particles[i];

                    if (particle.isPlaying)
                        continue;
                    
                    particle.transform.position = position;
                    particle.gameObject.SetActive(true);
                    particle.Play();
                    break;
                }
            }

            public void Spawn(Vector3 position, Transform parent)
            {
                for (int i = 0; i < _particles.Count; i++)
                {
                    ParticleSystem particle = _particles[i];

                    if (particle.isPlaying)
                        continue;
                    
                    particle.transform.position = position;
                    particle.transform.SetParent(parent);
                    particle.gameObject.SetActive(true);
                    particle.Play();

                    break;
                }
            }            
        }
    }
}