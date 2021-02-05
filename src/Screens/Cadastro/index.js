import React, { useState, useContext } from 'react';
import { useNavigation } from '@react-navigation/native';
import { Container, InputArea, CustomButton, CustomButtonText, SignMessageButton, SignMessageButtonText, SignMessageButtonTextBold } from './styles';

//import CashlogLogo from '../../Assets/cashlogLogo.png';
// Se quiser colocar alguma imagem:
//<CashlogLogo width="100%" height="160" />

import CadastroInput from '../../Components/CadastroInput';
import API from '../../API';
import AsyncStorage from '@react-native-community/async-storage';
import { UserContext } from '../../Contexts/UserContext';

export default () => {

    const[emailField, setEmailField] = useState('');
    const[senhaField, setSenhaField] = useState('');
    const[confirmarSenhaField, setConfirmarSenhaField] = useState('');
    const[cpfField, setCpfField] = useState('');

    const navigation = useNavigation();
    const { dispatch: userDispatch} = useContext(UserContext);

    const handleLoginClick = async () => {
        if (emailField !='' && senhaField !='' && confirmarSenhaField !='' && cpfField !='') {
            let res = await API.cadastro(emailField, senhaField, confirmarSenhaField, cpfField);
            if(res.token) {
                await AsyncStorage.setItem('token', res.token);

                userDispatch({
                    type: 'setEmail, setSenha, setConfirmarSenha, setCpf',
                    payload:{
                        email: res.data.email,
                        senha: res.data.senha,
                        confirmarSenha: res.data.confirmarSenha,
                        cpf: res.data.cpf
                    }
                });

                navigation.reset({
                    routes: [{name: 'Login'}]
                });
            } else {
                alert("Email ou senha errados");
            }
        } else {
            alert("Preencha os Campos!");
        }
    }

    const handleMessageButtonClick = () => {
        navigation.reset({
            routes: [{name: 'Login'}]
        });
    }

    return (
        <Container>


            <InputArea>
                <CadastroInput placeholder="Digite seu Email" />
                <CadastroInput placeholder="Digite sua Senha" />
                <CadastroInput placeholder="Confirme sua Senha" />
                <CadastroInput placeholder="Digite seu CPF" />


                <CustomButton onPress={handleMessageButtonClick}>
                    <CustomButtonText>CADASTRAR</CustomButtonText>
                </CustomButton>
            </InputArea>

            <SignMessageButton onPress={handleLoginClick}>
                <SignMessageButtonText>Já possui uma conta?</SignMessageButtonText>
                <SignMessageButtonTextBold>Faça Login</SignMessageButtonTextBold>
            </SignMessageButton>
        </Container>
    );
}
