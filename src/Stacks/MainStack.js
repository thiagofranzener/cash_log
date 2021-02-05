import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';

import Cadastro from '../Screens/Cadastro';
import Login from '../Screens/Login';
import MainTab from '../Stacks/MainTab';

const Stack = createStackNavigator();

export default () => (
    <Stack.Navigator>
        <Stack.Screen name="Login" component={Login} />
        <Stack.Screen name="Cadastro" component={Cadastro} />
        <Stack.Screen name="MainTab" component={MainTab} />
    </Stack.Navigator>
);