using System.ComponentModel;

namespace ProductChallenge.Common;

public static class BindingListExtensions
{
    /// <summary>
    /// Suspender as notificações durante a carga troca N eventos por um só, evitando o piscar
    /// do DataGridView.
    /// </summary>
    public static void ReplaceAll<T>(this BindingList<T> list, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(items);

        var raiseEvents = list.RaiseListChangedEvents;
        list.RaiseListChangedEvents = false;

        try
        {
            list.Clear();

            foreach (var item in items)
            {
                list.Add(item);
            }
        }
        finally
        {
            list.RaiseListChangedEvents = raiseEvents;
            list.ResetBindings();
        }
    }
}
