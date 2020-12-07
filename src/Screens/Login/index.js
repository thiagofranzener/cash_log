import React from 'react';
import { useNavigation } from '@react-navigation/native';
import { Container, InputArea, CustomButton, CustomButtonText, SignMessageButton, SignMessageButtonText, SignMessageButtonTextBold } from './styles';

//import CashlogLogo from '../../Assets/cashlogLogo.png';
// Se quiser colocar alguma imagem:
//<CashlogLogo width="100%" height="160" />

import LoginInput from '../../Components/LoginInput';

export default () => {

    const navigation = useNavigation();

    const handleLoginClick = () => {
        navigation.reset({
            routes: [{name: 'MainTab'}]
        });
    }

    const handleMessageButtonClick = () => {
        navigation.reset({
            routes: [{name: 'Cadastro'}]
        });
    }

    return (
        <Container>
            <InputArea>
                <LoginInput placeholder="Digite seu Email" />

                <LoginInput placeholder="Digite sua Senha" />

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
