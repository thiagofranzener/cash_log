import React, { useState, useContext } from 'react';
import { useNavigation } from '@react-navigation/native';
import { Container, InputArea, CustomButton, CustomButtonText, SignMessageButton, SignMessageButtonText, SignMessageButtonTextBold } from './styles';
import { API } from '../../API';
import AsyncStorage from '@react-native-community/async-storage';

import { UserContext } from '../../Contexts/UserContext';

//import CashlogLogo from '../../Assets/cashlogLogo.png';
// Se quiser colocar alguma imagem:
//<CashlogLogo width="100%" height="160" />

import LoginInput from '../../Components/LoginInput';

export default () => {

    const[emailField, setEmailField] = useState('');
    const[senhaField, setSenhaField] = useState('');

    const navigation = useNavigation();
    const { dispatch: userDispatch} = useContext(UserContext);

    const handleLoginClick = async() => {
        if(emailField !='' && senha != ''){

            let res = await API.login(emailField, senhaField);

            if(res.token) {
                await AsyncStorage.setItem('token', json.token);

                userDispatch({
                    type: 'setEmail, setSenha',
                    payload:{
                        email: JSON.data.email,
                        senha: JSON.data.senha
                    }
                });

                navigation.reset({
                    routes: [{name: 'MainTab'}]
                });
            } else {
                alert("Email ou senha errados")
            }
        } else {
            alert("Preencha os campos!");
        }
    }

    const handleMessageButtonClick = () => {
        navigation.reset({
            routes: [{name: 'Cadastro'}]
        });
    }

    return (
        <Container>
            <InputArea>
                <LoginInput 
                    placeholder="Digite seu Email"
                    value={emailField}
                    onChangeText={t=>setEmailField(t)}
                />

                <LoginInput 
                    placeholder="Digite sua Senha"
                    value={senhaField}
                    onChangeText={t=>setSenhaField(t)}
                    senha={true}
                />

                <CustomButton onPress={handleLoginClick}>
                    <CustomButtonText>LOGIN</CustomButtonText>
                </CustomButton>
            </InputArea>

            <SignMessageButton onPress={handleMessageButtonClick}>
                <SignMessageButtonText>Ainda não possui uma conta?</SignMessageButtonText>
                <SignMessageButtonTextBold>Cadastre-se</SignMessageButtonTextBold>
            </SignMessageButton>
        </Container>
    );
}
