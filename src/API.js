const BASE_API = ''; //INTEGRAR A API AQUI

export default {
    login: async (email, senha) => {
        const req = await fetch(`${BASE_API}/auth/login`, {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({email, senha})
        });
        const json = await req.json();
        return json;
    },
    cadastro: async (email, senha, confirmarSenha, cpf) => {
        const req = await fetch(`${BASE_API}/cadastro`, {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({email, senha, confirmarSenha, cpf})
        });
        const json = await req.json();
        return json;
    }
};