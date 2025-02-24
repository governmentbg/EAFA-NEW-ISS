using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public abstract class BaseAssignableModel<T> : BaseModel where T : class
    {
        public void AssignFrom(T source)
        {
            if (source == null) return;

            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(source));
                }
            }
        }

        public T DeepClone()
        {
            var json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
