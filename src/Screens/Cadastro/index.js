import React from 'react';
import { useNavigation } from '@react-navigation/native';
import { Container, InputArea, CustomButton, CustomButtonText, SignMessageButton, SignMessageButtonText, SignMessageButtonTextBold } from './styles';

//import CashlogLogo from '../../Assets/cashlogLogo.png';
// Se quiser colocar alguma imagem:
//<CashlogLogo width="100%" height="160" />

import SignInput from '../../Components/LoginInput'
import LoginInput from '../../Components/LoginInput';

export default () => {

    const navigation = useNavigation();

    const handleLoginClick = () => {
        navigation.reset({
            routes: [{name: 'Login'}]
        });
    }

    const handleMessageButtonClick = () => {
        navigation.reset({
            routes: [{name: 'Login'}]
        });
    }

    return (
        <Container>


            <InputArea>
                <LoginInput placeholder="Digite seu Email" />
                <LoginInput placeholder="Digite sua Senha" />
                <LoginInput placeholder="Confirme sua Senha" />
                <LoginInput placeholder="Digite seu CPF" />


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
