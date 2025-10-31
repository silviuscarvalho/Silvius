(function () {
    const connectButton = document.getElementById('connectWallet');
    const walletAddressDisplay = document.getElementById('walletAddress');
    const walletInput = document.getElementById('WalletAddress');
    const paymentForm = document.getElementById('paymentForm');
    const directionInput = document.getElementById('Direction');
    const fiatInput = document.getElementById('FiatAmount');
    const usdtInput = document.getElementById('UsdtAmount');
    const feeInput = document.getElementById('ServiceFee');
    const networkFeeInput = document.getElementById('NetworkFee');
    const exchangeRateInput = document.getElementById('ExchangeRate');
    const transactionsTableBody = document.getElementById('transactionsBody');

    if (!connectButton) {
        return;
    }

    const phantom = window.solana && window.solana.isPhantom ? window.solana : null;
    const metamask = window.ethereum ? window.ethereum : null;

    const storageKey = 'silvius-wallet';

    function setWallet(address) {
        walletInput.value = address;
        walletAddressDisplay.textContent = address;
        connectButton.textContent = 'Trocar carteira';
        localStorage.setItem(storageKey, address);
        fetchTransactions(address);
    }

    async function connect() {
        try {
            if (phantom) {
                const response = await phantom.connect();
                if (response.publicKey) {
                    setWallet(response.publicKey.toString());
                    return;
                }
            }
            if (metamask) {
                const accounts = await metamask.request({ method: 'eth_requestAccounts' });
                if (accounts && accounts.length) {
                    setWallet(accounts[0]);
                    return;
                }
            }
            alert('Nenhuma carteira Phantom ou MetaMask encontrada. Instale uma extensão compatível.');
        } catch (error) {
            console.error('Erro ao conectar carteira', error);
            alert('Não foi possível conectar à carteira.');
        }
    }

    async function fetchTransactions(address) {
        try {
            const response = await fetch(`/api/transactions?wallet=${encodeURIComponent(address)}`);
            if (!response.ok) {
                throw new Error('Erro ao carregar transações');
            }
            const data = await response.json();
            renderTransactions(data);
        } catch (error) {
            console.error(error);
        }
    }

    function renderTransactions(transactions) {
        transactionsTableBody.innerHTML = '';
        if (!transactions || !transactions.length) {
            transactionsTableBody.innerHTML = `<tr><td colspan="8">Nenhuma transação encontrada.</td></tr>`;
            return;
        }

        for (const tx of transactions) {
            const statusClass = tx.status === 'Completed' ? 'completed' : (tx.status === 'Failed' ? 'failed' : 'pending');
            const receiptIcon = tx.receipt ? `<a class="link-icon" href="/api/transactions/${tx.id}/receipt" title="Baixar comprovante"><i class="fa-solid fa-file-arrow-down"></i></a>` : '';
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${new Date(tx.createdAt).toLocaleString()}</td>
                <td>${tx.direction === 'UsdtToBrl' ? 'USDT → BRL' : 'BRL → USDT'}</td>
                <td>${Number(tx.requestedAmountUsdt).toFixed(2)} USDT</td>
                <td>R$ ${Number(tx.requestedAmountFiat).toFixed(2)}</td>
                <td>${Number(tx.serviceFeeFiat).toFixed(2)}</td>
                <td>${Number(tx.networkFee).toFixed(2)}</td>
                <td><span class="status-chip ${statusClass}">${tx.status}</span></td>
                <td>${receiptIcon}</td>
            `;
            transactionsTableBody.appendChild(row);
        }
    }

    async function recalculateQuote() {
        const direction = directionInput.value;
        const amount = direction === 'UsdtToBrl'
            ? Number(usdtInput.value || 0)
            : Number(fiatInput.value || 0);
        const wallet = walletInput.value;
        if (!wallet || !direction || !amount) {
            return;
        }
        const formData = new FormData(paymentForm);
        const response = await fetch(paymentForm.dataset.quoteUrl, {
            method: 'POST',
            body: formData,
        });
        if (response.ok) {
            const quote = await response.json();
            fiatInput.value = quote.fiatAmount.toFixed(2);
            usdtInput.value = quote.usdtAmount.toFixed(2);
            feeInput.value = quote.serviceFee.toFixed(2);
            networkFeeInput.value = quote.networkFee.toFixed(2);
            exchangeRateInput.value = quote.exchangeRate.toFixed(4);
        }
    }

    connectButton.addEventListener('click', async (event) => {
        event.preventDefault();
        await connect();
    });

    directionInput?.addEventListener('change', () => {
        if (directionInput.value === 'UsdtToBrl') {
            fiatInput.value = '';
        } else {
            usdtInput.value = '';
        }
        recalculateQuote();
    });

    paymentForm?.addEventListener('change', () => {
        recalculateQuote();
    });

    paymentForm?.addEventListener('submit', async (event) => {
        event.preventDefault();
        if (!walletInput.value) {
            alert('Conecte uma carteira antes de enviar.');
            return;
        }
        const formData = new FormData(paymentForm);
        const response = await fetch(paymentForm.action, {
            method: 'POST',
            body: formData
        });
        if (response.ok) {
            const transaction = await response.json();
            fetchTransactions(walletInput.value);
            alert('Transação criada e enviada para processamento.');
        } else {
            const error = await response.text();
            alert(error);
        }
    });

    const savedWallet = localStorage.getItem(storageKey);
    if (savedWallet) {
        setWallet(savedWallet);
    }
})();
