import styled from 'styled-components';

export const Container = styled.View`
  flex: 1;
  justifi-content: center;
  align-items: center;
  background-color: #49265c;
`;

export const Title = styled.Text`
  font-size: 20px;
  color: #ff8b0d;
`

export const Input = styled.TextInput`
  width: 90%;
  height: 50px;
  border-radius: 10px;
  background: #fff;
  padding: 10px;
  margin: 20px;
`

export const Button = styled.TouchableOpacity`
  width: 50%;
  height: 50px;
  background: transparent;
  align-items: center;
  justify-content: center;
  margin: 20px;
  border-radius: 10px;
`

export const TextButton = styled.Text`
  color: white;
`