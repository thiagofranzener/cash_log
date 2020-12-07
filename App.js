import React from 'react';
import { NavigationContainer } from '@react-navigation/native';

import MainStack from './src/Stacks/MainStack';

export default () => {
  return(
    <NavigationContainer>
      <MainStack />
    </NavigationContainer>
  );
}