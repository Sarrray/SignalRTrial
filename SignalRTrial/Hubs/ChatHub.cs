using Microsoft.AspNetCore.SignalR;
using SignalRTry.Models;
using SignalRTry.Services;
using System.Collections.Concurrent;

namespace SignalRTry.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserConnectionService _userConnectionService;

        public ChatHub(UserConnectionService userConnectionService)
        {
            _userConnectionService = userConnectionService;
        }

        // 接続時
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        // 切断時
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // 切断された接続をリストから削除
            _userConnectionService.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        // 接続している全ユーザーにメッセージを送信
        public async Task NewMessage(string username, string message) =>
            await Clients.All.SendAsync("messageReceived", username, message, DateTime.Now.ToString());


        // 接続しているユーザーの一覧を返す
        public async Task<IEnumerable<string>> GetUserList()
        {
            return await Task.FromResult(_userConnectionService.GetConnectedUsers());
        }

        // ユーザー名を設定
        public Task<SetUserResultData> SetUsername(string username)
        {
            // ユーザー名をチェック＆再設定
            long longRegisterUsername = long.TryParse(username, out long i) ? i : 1;
            bool isNameOK = false;
            // 他のユーザーと名前が重複する場合は+1したものを再設定
            while (!isNameOK)
            {
                if (_userConnectionService.IsValidUsername(longRegisterUsername.ToString().PadLeft(username.Length, '0'), out _))
                {
                    isNameOK = true;
                    break;
                }

                longRegisterUsername += 1;
            }

            // ユーザー名を設定
            string registerUsername = longRegisterUsername.ToString().PadLeft(username.Length, '0');
            _userConnectionService.AddConnection(Context.ConnectionId, registerUsername);

            // OKを返す
            return Task.FromResult(new SetUserResultData() { isSuccess = true, yourname= registerUsername });
        }

        // 金額を設定
        public async Task SendContent(string price)
        {
            string username = _userConnectionService.GetUsernameFromConnectionId(Context.ConnectionId);
            if (decimal.TryParse(price, out var decPrice) && decPrice >= 0)
            {
                await Clients.All.SendAsync("ReceiveContent", username, price, DateTime.Now.ToString());
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveError", "0以上の数値で入力してください");

            }
        }
    }
}
