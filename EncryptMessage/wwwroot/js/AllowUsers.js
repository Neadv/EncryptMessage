let userListItems = [];

document.addEventListener("DOMContentLoaded", () => {
    const users = document.getElementById("users");
    const private = document.getElementById("IsPrivate");

    let updateVisible = () => {
        if (private.checked) {
            users.style.display = "block";
        }
        else {
            users.style.display = "none";
        }
    };

    private.onchange = updateVisible;

    const usersList = document.getElementById("usersList");
    const username = document.getElementById("username");
    const addButton = document.getElementById("addUserButton");

    addButton.onclick = async () => {
        username.value = username.value.trim();
        if (checkAddedUsers(username.value, usersList)) {
            let result = await checkUser(username.value);
            if (result) {
                let user = new UserListItem(usersList.children.length, username.value, removeAllowUser);
                userListItems.push(user);

                user.render(usersList);
            }
        }
        username.value = "";
    };

    updateVisible();
    updateList(usersList);
});

function checkAddedUsers(username) {
    for (let u of userListItems) {
        if (u.username == username)
            return false;
    }
    return true;
}

async function checkUser(username) {
    let response = await fetch('/user/checkuser', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(username)
    });

    return await response.json();
}

async function updateList(parent) {
    parent = parent.querySelector("tbody") ?? parent;
    let oldItems = [];
    for (let i = 0; i < parent.children.length; i++){
        let input = parent.children[i].querySelector("input");
        userListItems.push(new UserListItem(i, input.value, removeAllowUser));
        oldItems.push(parent.children[i]);
    }

    oldItems.forEach(i => i.remove());

    for (let u of userListItems) {
        u.render(parent);
    }
}

function removeAllowUser(removedItem) {
    userListItems.splice(removedItem.number, 1);

    for (var i = 0; i < userListItems.length; i++) {
        userListItems[i].clear();
        userListItems[i].number = i;
    }

    for (let item of userListItems) {
        item.render(document.getElementById("usersList"));
    }
}

class UserListItem {
    constructor(number, username, removeCallback) {
        this.id = "Item";
        this.name = "Users";
        this.number = number;
        this.username = username;
        this.removeCallback = removeCallback;
    }

    render(parent) {
        let row = document.createElement("tr");
        row.id = this.id + this.number;

        let cell1 = document.createElement("td");
        let input = document.createElement("input");
        input.classList.add("form-control");
        input.type = "text";
        input.readOnly = true;
        input.name = `${this.name}[${this.number}]`;
        input.value = this.username;
        cell1.appendChild(input);

        let cell2 = document.createElement("td")
        let btn = document.createElement("button");
        btn.classList.add("btn", "btn-outline-danger");
        btn.type = "button";
        btn.innerText = "Remove";
        btn.onclick = () => {
            this.clear();
            this.removeCallback(this);
        }
        cell2.appendChild(btn);

        row.appendChild(cell1);
        row.appendChild(cell2);

        parent.appendChild(row);
    }

    clear() {
        let item = document.getElementById(this.id + this.number);
        item.remove();
    }
}