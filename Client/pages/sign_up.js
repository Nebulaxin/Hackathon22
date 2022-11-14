function sendJSON() {
    let name = document.querySelector('#Name');
//    let email = document.querySelector('#Email');
    let password = document.querySelector('#Password');
    let password2 = document.querySelector('#Password2');
    let result = document.querySelector('')
    let xhr = new XMLHttpRequest();
    let url = "http://185.177.218.151:5050/test";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
      if (xhr.readyState === 4 && xhr.status === 200) {
        // выводим то, что ответил нам сервер — так мы убедимся, что данные он получил правильно
        result.innerHTML = 123;
      }
    };
    var data = JSON.stringify({ "name": name.value,
     "password": password.value});
    xhr.send(data);
  }