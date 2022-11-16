function sendJSON() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    let sign_in = document.querySelector('.username');
    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/getProfile?data=" + encodeURIComponent(JSON.stringify({"token": localStorage.getItem('token')}));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
  
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0){
                sign_in.innerHTML = json.username;
            }
  
        }
    };
    xhr.send();

    let name = document.querySelector('#name');
    var url = "http://185.177.218.151:5050/getCards?data=" + encodeURIComponent(JSON.stringify({"token": localStorage.getItem('token'), "id": id}));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
  
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0){
                name.innerHTML = json.name; 
            }
            else{
                alert('Данной страницы не существует')
            }
        }
    };
    xhr.send();
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
}

function createTask(){
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

function saveTask(){

    var todo = document.getElementById("todo");
    var taskName = document.getElementById("task-name").value;
    var description = document.getElementById("task-description").value;
    if (description.length > 192) {
        alert("Превышено макс количество символов в описании");
        return;
    }

    if (taskName.length > 12) {
        alert("Превышено макс количество символов в названии");
        return;
    }
    
    todo.innerHTML += `
    <div class="task" id="${taskName.toLowerCase().split(" ").join("")}" draggable="true" ondragstart="drag(event)" style="word-wrap: break-word;">
        <span style="font-size: 30px;">${taskName}</span><br/>
        <span>${description}</span>
    </div>
    `
}

