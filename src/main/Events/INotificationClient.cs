using System.Threading;
using System.Threading.Tasks;

namespace org.neurul.Common.Events
{
    public interface INotificationClient
    {
        Task<NotificationLog> GetNotificationLog(string notificationLogId, CancellationToken token = default(CancellationToken));
    }
}
