using AppointmentBookingApp.Exceptions;
using AppointmentBookingApp.Interfaces;
using AppointmentBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Repositories
{
    public abstract class BaseRepository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly List<T> _items = new List<T>();
        public abstract K GenerateID();
        public abstract T GetById(K id);
        public abstract ICollection<T> GetAll();
        public T Create(T item)
        {
            var id = GenerateID();
            var property = typeof(T).GetProperty("Id");
            if (property != null)
            {
                property.SetValue(item, id);
            }
            if (_items.Contains(item))
            {
                throw new DuplicationEntryException("Appointment Already Exists.");
            }
            _items.Add(item);
            return item;
        }
        public T Update(T item)
        {
            var id = GetById((K)item.GetType().GetProperty("Id").GetValue(item));
            if (id == null) throw new KeyNotFoundException("Appointment not found.");
            var index = _items.IndexOf(id);
            if (index == -1) throw new KeyNotFoundException("Appointment not found.");
            _items[index] = item;
            return item;
        }
        public T Delete(K id)
        {
            var item = GetById(id);
            if (item == null) throw new KeyNotFoundException("Appointment not found.");
            _items.Remove(item);
            return item;
        }
    }
}
