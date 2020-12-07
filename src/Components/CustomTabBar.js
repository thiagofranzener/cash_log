import React from 'react';
import styled from 'styled-components/native';
import { Text } from 'react-native';

const TabArea = styled.View`
    height: 60px;
    background-color: #4EADBE;
    flex-direction: row;
`;

const TabItem = styled.TouchableOpacity`
    flex: 1;
    justify-content: center;
    align-items: center;
`;

const TabItemCenter = styled.TouchableOpacity`
    width: 70px;
    height: 70px;
    justify-content: center;
    align-items: center;
    background-color: #FFF;
    border-radius: 35px;
    border: 3px solid #4EADBE;
    margin-top: -20px;
`;

export default ({state, navigation}) => {

    const goTo = (screenName) => {
        navigation.navigate(screenName);
    }

    return(
        <TabArea>
            <TabItem onPress={()=>goTo('ConsultaDespesas')}>
                <Text>Consulta</Text>
            </TabItem>

            <TabItemCenter onPress={()=>goTo('CadastroDespesas')}>
                <Text>Cadastrar</Text>
            </TabItemCenter>

            <TabItem onPress={()=>goTo('AprovarDespesas')}>
                <Text>Aprovar Despesas</Text>
            </TabItem>

            <TabItem onPress={()=>goTo('PerfilUsuario')}>
                <Text>Perfil</Text>
            </TabItem>
        </TabArea>
    );
};