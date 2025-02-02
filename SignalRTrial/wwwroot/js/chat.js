"use strict";

const divMessages = document.querySelector("#divMessages");
const tbKingaku = document.querySelector("#tbKingaku");
const btnSend = document.querySelector("#btnSend");
const divErrorMessage = document.querySelector("#divErrorMessage");
const ulUserList = document.querySelector("#ulUserList");
const tbMyUsername = document.querySelector("#tbMyUsername");
const btnSetMyUsrname = document.querySelector("#btnSetMyUsrname");

const connection = new signalR.HubConnectionBuilder().withUrl("/hub").build();

// ロード時
window.addEventListener("load", () => {
	connection.start()
		.then(async () => {
			const myusername = await connection.invoke("SetUsername", getCurrentTimeMMddhhmmss());
			console.log(myusername);
			if (myusername.yourname) {
				tbMyUsername.value = myusername.yourname;
			}
		})
		.catch(() => console.error("connection error"));

	connection.on("ReceiveConnectedUsers", (usernames) => setUlUserList(usernames));

	connection.on("ReceiveContent", (username, message, datetime) => {
		const m = document.createElement("div");

		m.innerHTML = `${datetime} --> ${username }が<span class="message-text">${message}</span>に変更`;

		divMessages.appendChild(m);
		divMessages.scrollTop = divMessages.scrollHeight;
	});

	connection.on("ReceiveError", (message) => {
		divErrorMessage.innerText = message;
		divErrorMessage.style.display = "";
	});
});

// ユーザー名の入力制御
tbMyUsername.addEventListener("input", (e) => {
	const value = e.target.value;
	if (value.length > 16) {
		alert("名前は16桁以内としてください。");
		e.target.value = value.slice(0, 16);
	}
});

// 金額のテキストボックス内でEnter押下
tbKingaku.addEventListener("keyup", (e) => {
	if (e.key === "Enter") {
		sendKingaku();
	}
});

// 金額送信ボタン押下
btnSend.addEventListener("click", () => {
	sendKingaku();
})

// 金額送信
const sendKingaku = () => {
	divErrorMessage.innerText = "";
	divErrorMessage.style.display = "none";
	connection.send("SendContent", tbKingaku.value)
		.then(() => (tbKingaku.value = ""));
}

// 名前設定ボタン押下
btnSetMyUsrname.addEventListener("click", () => {
	sendUsername();
});

// 名前のテキストボックス内でEnter押下
tbMyUsername.addEventListener("keyup", (e) => {
	if (e.key === "Enter") {
		sendUsername();
	}
});

// 名前送信
const sendUsername = () => {
	const myusername = tbMyUsername.value;
	if (myusername.trim()) {
		connection.send("SetUsername", myusername);
	}
}

// ユーザー一覧を画面に設定
const setUlUserList = (usernames) => {
	ulUserList.innerHTML = "";
	usernames.forEach(username => {
		const li = document.createElement("li");
		li.textContent = username;
		ulUserList.appendChild(li);
	});
}

// 現在時刻をMMddhhmmss形式で取得
const getCurrentTimeMMddhhmmss = () => {
	const now = new Date();

	const month = String(now.getMonth() + 1).padStart(2, '0');
	const day = String(now.getDate()).padStart(2, '0');
	const hours = String(now.getHours()).padStart(2, '0');
	const minutes = String(now.getMinutes()).padStart(2, '0');
	const seconds = String(now.getSeconds()).padStart(2, '0');

	return `${month}${day}${hours}${minutes}${seconds}`;
}
