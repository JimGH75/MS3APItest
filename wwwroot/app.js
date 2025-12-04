// =============================================================
//  LOAD LINES (AGGREGATED)
// =============================================================
async function loadLines() {
    const resp = await fetch("/api/lines");
    const data = await resp.json();

    const container = document.getElementById("lines-container");
    container.innerHTML = "";

    data.forEach(line => {
        const div = document.createElement("div");
        div.className = "line-box";

        div.innerHTML = `
            <div class="line-title">${line.name}</div>
            <div class="progress">
                <div class="progress-bar" style="width:${line.pct}%">${line.pct}%</div>
            </div>
            <div class="line-info">
                Objednávky: ${line.ordersCount} <br>
                Váha: ${line.totalKg} kg
            </div>
        `;

        container.appendChild(div);
    });
}

// =============================================================
//  LOAD ORDERS GROUPED BY LINE
// =============================================================
async function loadOrders() {
    const resp = await fetch("/api/orders/grouped");
    const data = await resp.json();

    const ordersContainer = document.getElementById("orders-container");
    const unassignedContainer = document.getElementById("unassigned-container");

    ordersContainer.innerHTML = "";
    unassignedContainer.innerHTML = "";

    // GROUPED BY LINE
    Object.keys(data).forEach(l => {
        if (l === "UNASSIGNED") return;

        const block = document.createElement("div");
        block.className = "orders-block";

        block.innerHTML = `
            <h3>Linka ${l}</h3>
            <table class="orders-table">
                <thead>
                    <tr>
                        <th>Datum</th>
                        <th>Firma</th>
                        <th>Město</th>
                        <th>Cena</th>
                        <th>Váha</th>
                    </tr>
                </thead>
                <tbody id="tbl-${l}"></tbody>
            </table>
        `;

        ordersContainer.appendChild(block);

        const tbody = block.querySelector("tbody");

        data[l].forEach(o => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${o.date}</td>
                <td><a href="#" class="order-link" data-id="${o.id}">${o.company}</a></td>
                <td>${o.city}</td>
                <td>${o.totalPrice} Kč</td>
                <td>${o.totalKg} kg</td>
            `;
            tbody.appendChild(tr);
        });
    });

    // UNASSIGNED
    if (data.UNASSIGNED) {
        const list = data.UNASSIGNED.sort((a, b) => a.city.localeCompare(b.city));

        list.forEach(o => {
            const tr = document.createElement("div");
            tr.className = "unassigned-item";

            tr.innerHTML = `
                <a href="#" class="order-link" data-id="${o.id}">
                    ${o.city} – ${o.company} (váha ${o.totalKg} kg)
                </a>
            `;
            unassignedContainer.appendChild(tr);
        });
    }

    bindOrderLinks();
}

// =============================================================
//  ORDER DETAIL
// =============================================================
function bindOrderLinks() {
    const links = document.querySelectorAll(".order-link");

    links.forEach(link => {
        link.addEventListener("click", async e => {
            e.preventDefault();
            const id = link.dataset.id;

            const resp = await fetch(`/api/orders/${id}`);
            const detail = await resp.json();

            const panel = document.getElementById("order-detail");

            panel.innerHTML = `
                <h3>Objednávka ${detail.id}</h3>
                <p><strong>Firma:</strong> ${detail.company}</p>
                <p><strong>Město:</strong> ${detail.city}</p>
                <p><strong>Datum:</strong> ${detail.date}</p>
                <p><strong>Celkem váha:</strong> ${detail.totalKg} kg</p>
                <p><strong>Cena:</strong> ${detail.totalPrice} Kč</p>
                <h4>Položky:</h4>
                <table class="orders-table">
                    <thead>
                        <tr>
                            <th>Popis</th>
                            <th>Počet</th>
                            <th>MJ</th>
                            <th>Váha</th>
                            <th>Cena</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${detail.items.map(i => `
                            <tr>
                                <td>${i.description}</td>
                                <td>${i.count}</td>
                                <td>${i.unit}</td>
                                <td>${i.weight} kg</td>
                                <td>${i.price} Kč</td>
                            </tr>
                        `).join("")}
                    </tbody>
                </table>
            `;
        });
    });
}

loadLines();
loadOrders();
