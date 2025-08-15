// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(Collider))]
// public class RhythmZone : MonoBehaviour
// {
//     [Header("Links")]
//     [SerializeField] private SoundManager soundManager;
//     [SerializeField] private AudioSource zoneMaster; // основная мелодия для этой зоны

//     [Header("Tempo & Session")]
//     [SerializeField] private double zoneBpm = 120.0;
//     [Tooltip("Если true: при входе запускаем НОВУЮ сессию (бит 0), выравнивая master к биту 0). " +
//              "Если false: просто подключаем эмиттеры к уже идущей сессии.")]
//     [SerializeField] private bool startNewSessionOnEnter = true;

//     [Header("Exit Behaviour")]
//     [Tooltip("Удалять эмиттеры и останавливать их лупы при выходе из зоны.")]
//     [SerializeField] private bool cleanupOnExit = true;
//     [Tooltip("Остановить ли основной трек зоны при выходе.")]
//     [SerializeField] private bool stopMasterOnExit = true;

//     [Header("Player Filter")]
//     [SerializeField] private string playerTag = "Player";

//     // Кеш всех эмиттеров зоны (чтобы быстро добавлять/удалять)
//     private readonly List<ISoundEmitter> _zoneEmitters = new List<ISoundEmitter>();
//     private bool _active = false;

//     private void Reset()
//     {
//         var col = GetComponent<Collider>();
//         col.isTrigger = true; // важно!
//     }

//     private void Awake()
//     {
//         // Собираем эмиттеры, расположенные ВНУТРИ иерархии зоны (обычно они — дети зоны)
//         GetComponentsInChildren<ISoundEmitter>(includeInactive: true, result: _zoneEmitters);
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!other.CompareTag(playerTag)) return;
//         if (_active) return; // защита от повторных вызовов
//         _active = true;

//         if (startNewSessionOnEnter)
//         {
//             // Стартуем новую сессию — бит 0 в момент старта master.
//             StartNewSessionAlignedToZero();
//         }
//         else
//         {
//             // Подключаемся к текущей сессии. Если мастер не играет — запустим его по ближайшему следующему биту.
//             JoinExistingSession();
//         }

//         // Регистрируем эмиттеры зоны
//         foreach (var e in _zoneEmitters)
//             soundManager.Container.Add(e);
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (!other.CompareTag(playerTag)) return;
//         if (!_active) return;
//         _active = false;

//         if (cleanupOnExit)
//         {
//             // Убираем эмиттеры и гасим их лупы
//             foreach (var e in _zoneEmitters)
//             {
//                 soundManager.StopLoop(e);
//                 soundManager.Container.Remove(e);
//             }
//         }

//         if (stopMasterOnExit && zoneMaster != null)
//         {
//             zoneMaster.Stop();
//         }
//     }

//     private void StartNewSessionAlignedToZero()
//     {
//         if (soundManager == null) { Debug.LogWarning("RhythmZone: SoundManager not set."); return; }
//         if (zoneMaster == null)   { Debug.LogWarning("RhythmZone: zoneMaster not set.");  return; }

//         // “Перезапуск” — сбиваем внутренний счётчик и ставим master точно на бит 0.
//         // Для этого добавим удобный метод в SoundManager (см. ниже).
//         soundManager.StartSession(zoneBpm, zoneMaster);
//     }

//     private void JoinExistingSession()
//     {
//         if (soundManager == null) { Debug.LogWarning("RhythmZone: SoundManager not set."); return; }
//         if (zoneMaster == null)   { Debug.LogWarning("RhythmZone: zoneMaster not set.");  return; }

//         // Если мастер уже играет — оставляем как есть (эмиттеры подстроятся к текущему биту).
//         // Если нет — стартуем его по ближайшему следующему целому биту текущей сессии (чтобы ложился в сетку).
//         if (!zoneMaster.isPlaying)
//         {
//             var dspNow = AudioSettings.dspTime;
//             var beatNow = soundManager.CurrentBeat(dspNow);
//             var nextWholeBeat = System.Math.Ceiling(beatNow);
//             var startTime = soundManager.BeatToDsp(nextWholeBeat, dspNow) + soundManager.safetyLeadTime;
//             zoneMaster.PlayScheduled(startTime);
//         }
//     }
// }
