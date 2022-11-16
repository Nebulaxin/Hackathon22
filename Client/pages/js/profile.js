function sendJSON() {

  let sign_in = document.querySelector('.username');

  var xhr = new XMLHttpRequest();
  var url = "http://185.177.218.151:5050/getProfile?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token') }));
  xhr.open("GET", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");

  xhr.onreadystatechange = function () {
    if (xhr.readyState === 4 && xhr.status === 200) {
      var json = JSON.parse(xhr.responseText);
      if (json.status == 0) {
        sign_in.innerHTML = json.name + "/" + json.username;

        var todo = document.getElementById("desks");
        const desks = json.desks;

        for (var i = 0; i < desks.length; i++) {
          todo.innerHTML += `
                      <div class="card">
                      <h1>
                        <a id="a_card" href="desk_page.html?id=${desks[i].id}" style="color: black">${desks[i].name}</a>
                      </h1>
                    </div>
              `


        }
      }
      else if (json.status == 8) {
        alert("maximum desks per user: 4");
      }


    }
  };
  xhr.send();
}

function LogOut() {
  localStorage.removeItem('token');
  window.location.href = "../html/sign_in.html";
}

function SendDesk() {
  let title = document.querySelector('#Title');

  var xhr = new XMLHttpRequest();
  var url = "http://185.177.218.151:5050/createDesk?data=" + encodeURIComponent(JSON.stringify({ "token": localStorage.getItem('token'), "name": title.value }));
  xhr.open("GET", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");

  xhr.onreadystatechange = function () {
    if (xhr.readyState === 4 && xhr.status === 200) {
      var json = JSON.parse(xhr.responseText);
      if (json.status == 0) {
        window.location.href = "../html/desk_page.html?id=" + json.id;
      }
      else if (json.status == 8) {
        alert("maximum desks per user: 4");
      }
    }
  };
  xhr.send();
}



document.addEventListener("DOMContentLoaded", function () {
  var scrollbar = document.body.clientWidth - window.innerWidth + 'px';
  console.log(scrollbar);
  document.querySelector('[href="#openModal"]').addEventListener('click', function () {
    document.body.style.overflow = 'hidden';
    document.querySelector('#openModal').style.marginLeft = scrollbar;
  });
  document.querySelector('[href="#close"]').addEventListener('click', function () {
    document.body.style.overflow = 'visible';
    document.querySelector('#openModal').style.marginLeft = '0px';
  });
});

