using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiControlStyle : IDictionary<string, object>
    {
        private Dictionary<string, object> _previousState;

        public GuiControlStyle(Type targetType)
            : this(targetType.FullName, targetType)
        {
        }

        public GuiControlStyle(string name, Type targetType)
        {
            Name = name;
            TargetType = targetType;
            _setters = new Dictionary<string, object>();
        }

        public string Name { get; }
        public Type TargetType { get; set; }

        private readonly Dictionary<string, object> _setters;
        
        public void ApplyIf(GuiControl control, bool predicate)
        {
            if (predicate)
                Apply(control);
            else
                Revert(control);
        }

        public void Apply(GuiControl control)
        {
            _previousState = _setters
                .ToDictionary(i => i.Key, i => TargetType.GetRuntimeProperty(i.Key)?.GetValue(control));

            ChangePropertyValues(control, _setters);
        }

        public void Revert(GuiControl control)
        {
            if (_previousState != null)
                ChangePropertyValues(control, _previousState);

            _previousState = null;
        }

        private static void ChangePropertyValues(GuiControl control, Dictionary<string, object> setters)
        {
            var targetType = control.GetType();

            foreach (var propertyName in setters.Keys)
            {
                var propertyInfo = targetType.GetRuntimeProperty(propertyName);
                var value = setters[propertyName];

                if (propertyInfo != null)
                {
                    if(propertyInfo.CanWrite)
                        propertyInfo.SetValue(control, value);

                    // special case when we have a list of items as objects (like on a list box)
                    if (propertyInfo.PropertyType == typeof(List<object>))
                    {
                        var items = (List<object>)value;
                        var addMethod = propertyInfo.PropertyType.GetRuntimeMethod("Add", new[] { typeof(object) });

                        foreach (var item in items)
                            addMethod.Invoke(propertyInfo.GetValue(control), new[] {item});
                    }
                }
            }
        }

        public object this[string key]
        {
            get { return _setters[key]; }
            set { _setters[key] = value; }
        }

        public ICollection<string> Keys => _setters.Keys;
        public ICollection<object> Values => _setters.Values;
        public int Count => _setters.Count;
        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _setters.GetEnumerator();
        public void Add(string key, object value) => _setters.Add(key, value);
        public void Add(KeyValuePair<string, object> item) => _setters.Add(item.Key, item.Value);
        public bool Remove(string key) => _setters.Remove(key);
        public bool Remove(KeyValuePair<string, object> item) => _setters.Remove(item.Key);
        public void Clear() => _setters.Clear();
        public bool Contains(KeyValuePair<string, object> item) => _setters.Contains(item);
        public bool ContainsKey(string key) => _setters.ContainsKey(key);
        public bool TryGetValue(string key, out object value) => _setters.TryGetValue(key, out value);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }
    }
}