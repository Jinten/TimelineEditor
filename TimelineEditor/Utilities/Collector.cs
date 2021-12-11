using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.Utilities
{
    internal static class Collector
    {
        public static void Sync<DEST, SRC>(IList<DEST> dest, in SRC[] src, Func<DEST, SRC, bool> compare, Func<SRC, DEST> activator)
        {
            if (src.Length == 0)
            {
                foreach (var item in dest.OfType<IDisposable>())
                {
                    item.Dispose();
                }

                dest.Clear();

                return;
            }

            for (int i = 0; i < src.Length; i++)
            {
                var item = src[i];
                if(dest.Count < i)
                {
                    if(compare(dest[i], item) == false)
                    {
                        var existingItem = dest.FirstOrDefault(arg => compare(arg, item));
                        if (existingItem != null)
                        {
                            // 既に存在するなら順序入れ替え
                            dest.Insert(i, existingItem);
                        }
                        else
                        {
                            // 既存アイテムがなければ、新規追加
                            dest.Insert(i, activator(item));
                        }
                    }
                }
                else
                {
                    // 既に上限数を超えている場合は、全て新規追加のはず
                    dest.Add(activator(item));
                }
            }

            // 上限を超えている要素数は、全て排除されているアイテム
            while(dest.Count > src.Length)
            {
                dest.RemoveAt(dest.Count - 1);
            }
        }
    }
}
