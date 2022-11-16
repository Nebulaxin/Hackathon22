function sendJSON() {
    let username = document.querySelector('#Username');
    let password = document.querySelector('#Password');

    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/signIn?data=" + encodeURIComponent(JSON.stringify({
        "username": username.value,
        "password": password.value
    }));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0) {
                localStorage.setItem('token', json.token);
                window.location.href = '../html/profile.html';
            }
            else if (json.status == 3) {
                alert("This username doesn't exist!");
            }
            else if (json.status == 4) {
                alert("Wrong password!");
            }
        }
    };
    xhr.send();
}