function sendJSON() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    var find = urlParams.has("findTag");
    let sign_in = document.querySelector('.username');

    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/getProfile?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token') }));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0) {
                sign_in.innerHTML = json.username;
            }
        };
    }

    xhr.send();

    if (find) {

    }
    else {

    }
    var xhr2 = new XMLHttpRequest();
    let name = document.querySelector('#name');
    var url = find
        ? "http://185.177.218.151:5050/getCardsByTag?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token'), "id": id, "tag": urlParams.get("findTag") }))
        : "http://185.177.218.151:5050/getCards?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token'), "id": id }));
    xhr2.open("GET", url, true);
    xhr2.setRequestHeader("Content-Type", "application/json");
    xhr2.onreadystatechange = function () {
        if (xhr2.readyState === 4 && xhr2.status === 200) {
            var json = JSON.parse(xhr2.responseText);
            if (json.status == 0) {
                name.innerHTML = json.name;
                var todo = document.getElementById("todo");
                var progress = document.getElementById("inprogress");
                var done = document.getElementById("done");
                const cards = json.cards;
                for (var i = 0; i < cards.length; i++) {
                    var totaltime = cards[i].expires - json.now;
                    var hours = Math.floor(totaltime / (1000 * 60 * 60));
                    var days = Math.floor(hours / 24);
                    hours %= 24;
                    var timeLeft = "" + (days == 0 ? "" : days + " days") + " " + hours + " hours";
                    const card = `
                    <div class="task" id="${cards[i].id}" draggable="true" ondragstart="drag(event)" style="word-wrap: break-word;">
                        <span style="font-size: 30px;">${cards[i].name}</span><br/>
                        <span>${cards[i].description}</span><br/><br/>
                        <span>Time: ${timeLeft}</span><br/>
                        <span>Tag: ${cards[i].tag}</span><br/>
                        <a id="${cards[i].id}" onclick="DeleteTask(event)" style="color: black; cursor: pointer; text-decoration: underline;">Delete</a>
                
                    </div>
                    `;
                    if (cards[i].status == "todo") {
                        todo.innerHTML += card
                    }
                    else if (cards[i].status == "inprogress") {
                        progress.innerHTML += card;
                    }
                    else if (cards[i].status == "done") {
                        done.innerHTML += card;
                    }
                }
            }
            else {
                alert('Данной страницы не существует')
                window.location.href = "../html/profile.html";
            }
        }
    };
    xhr2.send();
}


function LogOut() {
    localStorage.removeItem('token');
    window.location.href = "../html/sign_in.html";
}


function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function allowDrop(ev) {
    ev.preventDefault();
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.currentTarget.appendChild(document.getElementById(data));

    const urlParams = new URLSearchParams(window.location.search);

    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/moveCard?data=" + encodeURIComponent(JSON.stringify({ "id": parseInt(data), "status": ev.currentTarget.id }));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status != 0) {
                alert("Something went wrong!");
            }

        }
    };
    xhr.send();
}

function createTask() {
    var x = document.getElementById("inprogress");
    var y = document.getElementById("done");
    var z = document.getElementById("create-new-task-block");
    if (x.style.display === "none") {
        x.style.display = "block";
        y.style.display = "block";
        z.style.display = "none";
    } else {
        x.style.display = "none";
        y.style.display = "none";
        z.style.display = "flex";
    }
}

function saveTask() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    const tag = document.getElementById("task-tag").value;
    const name = document.getElementById("task-name").value;
    const time = document.getElementById("task-time").value;
    var description = document.getElementById("task-description").value;

    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/addCard?data=" + encodeURIComponent(JSON.stringify({
        "token": localStorage.getItem('token'), "id": parseInt(id), "tag": tag,
        "expires": parseInt(time), "description": description, "name": name
    }));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0) {
                var todo = document.getElementById("todo");
                var taskName = document.getElementById("task-name").value;
                var description = document.getElementById("task-description").value;
                var time = document.getElementById("task-time").value;
                var tag = document.getElementById("task-tag").value;

                if (description.length > 192) {
                    alert("Превышено макс количество символов в описании");
                    return;
                }

                if (taskName.length > 12) {
                    alert("Превышено макс количество символов в названии");
                    return;
                }

                todo.innerHTML += `
                <div class="task" id="${json.id}" draggable="true" ondragstart="drag(event)" style="word-wrap: break-word;">
                    <span style="font-size: 30px;">${taskName}</span><br/>
                    <span>${description}</span><br/><br/>
                    <span>Tag: ${tag}</span><br/>
                    <span>Time: ${time}d</span><br/>
                    <a id="${json.id}" onclick="DeleteTask(event)" style="color: black; cursor: pointer; text-decoration: underline;">Delete</a>
            
                </div>
                `
            }
            // else {
            //     LogOut();
            // }
        }
    };
    xhr.send();
}

function DeleteTask(ev) {
    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/deleteCard?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token'), "id": parseInt(ev.target.id) }));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0) {
                location.reload();
            }
            // else {
            //     LogOut();
            // }
        }
    };
    xhr.send();
}

function Find() {
    const urlParams = new URLSearchParams(window.location.search);
    var tag = document.querySelector(".input").value;

    urlParams.append("findTag", tag);

    window.location.href = "desk_page.html?" + urlParams.toString();


    // var xhr = new XMLHttpRequest();
    // var url = "http://185.177.218.151:5050/deleteCard?data=" + encodeURIComponent(JSON.stringify({ "id": parseInt(is), "tag": tag }));
    // xhr.open("GET", url, true);
    // xhr.setRequestHeader("Content-Type", "application/json");

    // xhr.onreadystatechange = function () {
    //     if (xhr.readyState === 4 && xhr.status === 200) {
    //         var json = JSON.parse(xhr.responseText);
    //         if (json.status == 0) {
    //             location.reload();
    //         }
    //         // else {
    //         //     LogOut();
    //         // }
    //     }
    // };
    // xhr.send();
}