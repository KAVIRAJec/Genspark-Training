let currentProductId = 1;
const productList = document.getElementById('product-list');
const addProductBtn = document.getElementById('addProductBtn');

function fetchProduct(id, callback) {
  fetch(`https://dummyjson.com/products/${id}`)
    .then(response => response.json())
    .then(product => callback(product))
    .catch(() => callback(null));
}

function addProductToDOM(product) {
  if (!product || !product.title) {
    const div = document.createElement('div');
    div.textContent = "No more products to add!";
    productList.appendChild(div);
    addProductBtn.disabled = true;
    return;
  }
  const div = document.createElement('div');
  div.className = "product-card";
  div.innerHTML = `
    <h3>${product.title}</h3>
    <img src="${product.thumbnail}" alt="${product.title}" />
    <p><b>Price:</b> $${product.price}</p>
    <p><b>Brand:</b> ${product.brand}</p>
    <p><b>Category:</b> ${product.category}</p>
    <p>${product.description}</p>
  `;
  productList.appendChild(div);
}

addProductBtn.onclick = function() {
  fetchProduct(currentProductId, (product) => {
    addProductToDOM(product);
    currentProductId++;
  });
};