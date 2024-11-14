
function addScore() {
    setTimeout(() => {
        const ads = document.querySelectorAll('.realty-preview-description__text');
        
        ads.forEach(ad => {
            if (!ad.querySelector('.sentiment-rating')) {
                const adText = ad.innerText;

                fetch('http://localhost:5175/api/sentiment/analyze', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ SentimentText: adText })
                })
                .then(response => response.json())
                .then(data => {
                    const score = data.score;
                    
                    // Визначаємо кількість заповнених зірок в залежності від оцінки
                    let filledStars = 0;
                    if (score >= 0 && score <= 0.2) {
                        filledStars = 1;
                    } else if (score > 0.2 && score <= 0.4) {
                        filledStars = 2;
                    } else if (score > 0.4 && score <= 0.6) {
                        filledStars = 3;
                    } else if (score > 0.6 && score <= 0.8) {
                        filledStars = 4;
                    } else if (score > 0.8 && score <= 1) {
                        filledStars = 5;
                    }

                    const ratingElement = document.createElement('span');
                    ratingElement.classList.add('sentiment-rating');
                    
                    // Створюємо зірки: заповнені або порожні
                    let starsHtml = '';
                    for (let i = 1; i <= 5; i++) {
                        if (i <= filledStars) {
                            starsHtml += `<span style="color: gold; font-size: 24px;">&#9733;</span>`; // Заповнена зірка
                        } else {
                            starsHtml += `<span style="color: lightgray; font-size: 24px;">&#9734;</span>`; // Порожня зірка
                        }
                    }
                    
                    // Додаємо зірки перед текстом оголошення
                    ratingElement.innerHTML = `${starsHtml}<br>`;
                    ad.innerHTML = ratingElement.outerHTML + ad.innerHTML;
                })
                .catch(error => console.error('Error:', error));
            }
        });
    }, 1500);
}

addScore();

var oldHref = document.location.href;



window.addEventListener("load", (event) => {
  var bodyList = document.querySelector('body');

    var observer = new MutationObserver(function(mutations) {
        if (oldHref != document.location.href) {
            oldHref = document.location.href;
            setTimeout(addScore(), 10000);
        }
    });
    
    var config = {
        childList: true,
        subtree: true
    };
    
    observer.observe(bodyList, config);
});

// Observe the body for changes in the child elements

console.log("page is fully loaded");