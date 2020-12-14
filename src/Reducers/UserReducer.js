export const initialState = {
    email: '',
    senha: '',
    confirmarSenha: '',
    cpf: '',
};

export const UserReducer = (state, action) => {
    switch(action.type) {
        case 'setEmail':
            return { ...state, email: action.payload.email };
        break;
        case 'setSenha':
            return { ...state, email: action.payload.senha };
        break;
        case 'setConfrimarSenha':
            return { ...state, email: action.payload.confirmarSenha };
        break;
        case 'setCpf':
            return { ...state, email: action.payload.cpf };
        break;
        default:
            return state;
    }
}