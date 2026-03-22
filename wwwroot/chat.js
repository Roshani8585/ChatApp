let connection;
let selectedUser = "";
let currentUser = "";
let isConnected = false;

// Invite
function inviteUser() {
    const email = document.getElementById("inviteEmail").value;

    fetch("/Invite/SendInvite", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(email)
    })
        .then(res => res.text())
        .then(data => alert(data))
        .catch(err => console.error(err));
}

// On Load
document.addEventListener("DOMContentLoaded", function () {

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    connection.start()
        .then(() => {
            console.log("Connected ✅");

            isConnected = true;
            document.getElementById("sendBtn").disabled = false;

            currentUser = prompt("Enter your name:");
            connection.invoke("SetUserName", currentUser);
        })
        .catch(err => console.error(err));

    // User List
    connection.on("UserList", (users) => {
        const ul = document.getElementById("userList");
        ul.innerHTML = "";

        users.forEach(user => {
            if (user !== currentUser) {
                const li = document.createElement("li");
                li.textContent = user;
                li.style.cursor = "pointer";
                li.onclick = () => selectUser(user);
                ul.appendChild(li);
            }
        });
    });

    // Receive Message
    connection.on("ReceivePrivateMessage", (user, message) => {
        const div = document.createElement("div");
        div.innerHTML = `<b>${user}:</b> ${message}`;
        document.getElementById("chatBox").appendChild(div);
    });
});

// Select user
function selectUser(user) {
    selectedUser = user;
    document.getElementById("chatWith").innerText = "Chat with " + user;
}

// Send Message
function sendPrivate() {

    if (!isConnected) {
        alert("Connection not ready");
        return;
    }

    if (!selectedUser) {
        alert("Select a user first");
        return;
    }

    const msg = document.getElementById("messageInput").value;

    if (!msg) {
        alert("Enter message");
        return;
    }

    connection.invoke("SendPrivateMessage", selectedUser, msg)
        .catch(err => console.error(err));

    document.getElementById("messageInput").value = "";
}


//connection.on("ReceivePrivateMessage", (user, message) => {
//    const div = document.createElement("div");
//    div.classList.add("message");
//    div.innerHTML = `<b>${user}:</b> ${message}`;
//    document.getElementById("chatBox").appendChild(div);
//});