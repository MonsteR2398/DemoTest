using System;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        // Основные данные (для сериализации)
        public List<SaveItem<int>> intData = new List<SaveItem<int>>();
        public List<SaveItem<long>> longData = new List<SaveItem<long>>();
        public List<SaveItem<float>> floatData = new List<SaveItem<float>>();
        public List<SaveItem<string>> stringData = new List<SaveItem<string>>();

        // Несериализуемые словари для быстрого доступа
        [NonSerialized] private Dictionary<string, int> _intCache = new Dictionary<string, int>();
        [NonSerialized] private Dictionary<string, long> _longCache = new Dictionary<string, long>();
        [NonSerialized] private Dictionary<string, float> _floatCache = new Dictionary<string, float>();
        [NonSerialized] private Dictionary<string, string> _stringCache = new Dictionary<string, string>();
        [NonSerialized] private bool _isCacheDirty = true;

        public void InitializeCache()
        {
            if (!_isCacheDirty) return;

            _intCache.Clear();
            foreach (var item in intData)
                _intCache[item.key] = item.value;
                
            _longCache.Clear();
            foreach (var item in longData)
                _longCache[item.key] = item.value;

            _floatCache.Clear();
            foreach (var item in floatData)
                _floatCache[item.key] = item.value;

            _stringCache.Clear();
            foreach (var item in stringData)
                _stringCache[item.key] = item.value;
            
            _isCacheDirty = false;
        }

        public int LoadInt(string name) => name != null ? _intCache.GetValueOrDefault(name) : int.MinValue;
        public long LoadLong(string name) => name != null ? _longCache.GetValueOrDefault(name) : long.MinValue;
        public float LoadFloat(string name) => name != null ? _floatCache.GetValueOrDefault(name) : float.NaN;
        public string LoadString(string name) => name != null ? _stringCache.GetValueOrDefault(name): null;

        
        public void SaveInt(string name, int value = 0)
        {
            _intCache[name] = value;
            UpdateListFromCache(ref intData, _intCache);
        }

        public void SaveLong(string name, long value = 0)
        {
            _longCache[name] = value;
            UpdateListFromCache(ref longData, _longCache);
        }

        public void SaveFloat(string name, float value = 0)
        {
            _floatCache[name] = value;
            UpdateListFromCache(ref floatData, _floatCache);
        }

        public void SaveString(string name, string value = "")
        {
            _stringCache[name] = value;
            UpdateListFromCache(ref stringData, _stringCache);
        }
        private void UpdateListFromCache<T>(ref List<SaveItem<T>> list, Dictionary<string, T> cache)
        {
            list.Clear();
            foreach (var kvp in cache)
                list.Add(new SaveItem<T> { key = kvp.Key, value = kvp.Value });
        }
    }
    [Serializable]
    public struct SaveItem<T>
    {
        public string key;
        public T value;
    }
}
