using System;
using System.Collections.Generic;
using System.Text; // для StringBuilder

namespace magazine._01
{
    public class LinearSearch
    {
        private List<MusicInstrument> items = new List<MusicInstrument>();

        public MusicInstrument Insert(MusicInstrument value)
        {
            items.Add(value);
            return value; // возвращаем добавленный элемент
        }


        public void Clear()
        {
            items.Clear();
        }

        // === ПОШУК ===
        public (string els, string log) Find(string name, string category)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder els = new StringBuilder();

            //sb.AppendLine($"Лінійний пошук '{name}' (категорія: '{category}')");

            int steps = 0;
            int matches = 0;

            for (int i = 0; i < items.Count; i++)
            {
                steps++;

                var item = items[i];
                if (item == null) continue;

                // защищаемся от null у полей
                string itemName = item.Name ?? string.Empty;
                string itemCategory = item.Category ?? string.Empty;
                string searchName = name ?? string.Empty;
                string searchCategory = category ?? string.Empty;

                if (itemName.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) >= 0
                    && itemCategory.IndexOf(searchCategory, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    matches++;

                    // Логируем каждый найденный элемент, но не выходим из цикла
                    sb.AppendLine($" -> ЗНАЙДЕНО: ID={item.Id}, Name=\"{item.Name}\", Category=\"{item.Category}\", Price={item.Price} (Індекс: {i}, Крок: {steps})");
                    els.AppendLine($"ЗНАЙДЕНО: {item.Name} (Ціна: {item.Price})");
                }
            }

            if (matches == 0)
            {
                sb.AppendLine("-> Не знайдено.");
            }
            else
            {
                sb.AppendLine($"-> Знайдено збігів: {matches}. Всього кроків: {steps}.");
            }

            return (els.ToString(), sb.ToString());
        }
    }
}