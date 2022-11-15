function sendJSON() {
    let name = document.querySelector('#Name');
//    let email = document.querySelector('#Email');
    let password = document.querySelector('#Password');
    let password2 = document.querySelector('#Password2');
    let xhr = new XMLHttpRequest();
    let url = "http://185.177.218.151:5050/singUp";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    var data = JSON.stringify({ "name": name.value,"username": "bebra",
     "password": password.value});
    xhr.send(data);
  }