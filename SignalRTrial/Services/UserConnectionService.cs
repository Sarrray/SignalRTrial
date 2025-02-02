using Microsoft.AspNetCore.SignalR;
using SignalRTry.Hubs;
using System.Collections.Concurrent;

namespace SignalRTry.Services
{
    public class UserConnectionService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ConcurrentDictionary<string, (string username, DateTime lastAccessTime)> _connections = new();

        public UserConnectionService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // ユーザーを追加する
        public void AddConnection(string connectionId, string username)
        {
            _connections[connectionId] = (username, DateTime.Now);
            NotifyClients();
        }

        // ユーザーを削除する
        public void RemoveConnection(string connectionId, bool isNotify = true)
        {
            if (isNotify && _connections.TryRemove(connectionId, out _))
            {
                NotifyClients();
            }
        }

        // 接続中のユーザー一覧を取得する
        public IEnumerable<string> GetConnectedUsers()
        {
            return _connections.Values.Select(x => x.username).Where(x => !string.IsNullOrEmpty(x.Trim()));
        }

        // 接続中のユーザー一覧を取得する
        public string GetUsernameFromConnectionId(string connectionId)
        {
            return _connections.FirstOrDefault(x => x.Key == connectionId).Value.username;
        }

        // ユーザーの一覧を通知する
        private void NotifyClients()
        {
            _hubContext.Clients.All.SendAsync("ReceiveConnectedUsers", GetConnectedUsers());
        }

        // 指定時間アクセスがない接続を削除する
        public void RemoveInactiveConnections(TimeSpan cleanupInterval)
        {
            var now = DateTime.Now;
            foreach (var connection in _connections)
            {
                if (now - connection.Value.lastAccessTime > cleanupInterval)
                {
                    RemoveConnection(connection.Key, false);
                }
            }
            NotifyClients();
        }

        // ユーザーの登録可否チェック
        public bool IsValidUsername(string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(username))
            {
                errorMessage = "ユーザー名を入力してください。";
                return false;
            }
            else if (GetConnectedUsers().Any(x => x == username))
            {
                errorMessage = "既に登録されているユーザーです。";
                return false;
            }

            return true;
        }
    }
}
