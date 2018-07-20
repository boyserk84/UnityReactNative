/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React, {Component} from 'react';
import {Platform, StyleSheet, Text, View, Button, ScrollView, TextInput, TouchableOpacity} from 'react-native';
import { Avatar, List, ListItem, Badge  } from 'react-native-elements'

const instructions = Platform.select({
  ios: 'Press Cmd+R to reload,\n' + 'Cmd+D or shake for dev menu',
  android:
    'Double tap R on your keyboard to reload,\n' +
    'Shake or press menu button for dev menu',
});


type Props = {};
export default class App extends Component<Props> {

  constructor(props) {
    super(props);
    this.state = { 
      isClick: true,
      title: "React-Native Chat!",
      message: "Message will show up here!",
      txtInput: ""
    };

    this.items = [];
    this.count = 0;

    this.onPressButton = this.onPressButton.bind(this);
  }

  onPressButton(e) {
    if (this.state.txtInput) {

      let newText = this.state.txtInput;
      this.count++;
      this.items.push({
        key: this.count,
        val: this.state.txtInput
      });

      this.setState({
        txtInput: ""
      })
    }
  }

  render() {
    let listItems = this.items.map((element, i) =>
       
         <Badge key={element.key} value={element.val} textStyle={{ color: 'orange' }}/>
        
     );

    let renderVar = 
    <ScrollView contentContainerStyle={styles.container}>
        <Text style={styles.welcome}>{this.state.title}</Text>
        <TextInput
          style={{ width: 250, height: 40, borderColor: '#b2bec3', borderWidth: 1}}
          placeholder="Type Anything here!"
          value={this.state.txtInput}
          onChangeText={(value)=> this.setState({ txtInput: value })}
        />

        <TouchableOpacity 
          onPress={this.onPressButton} 
          style={styles.button} >
          <Text>Touch to Send Message</Text>
        </TouchableOpacity>

        {listItems}

      </ScrollView>;

    return ( renderVar );

  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF'
  },
  welcome: {
    fontSize: 20,
    textAlign: 'center',
    margin: 10,
  },
  button: {
    alignItems: 'center',
    backgroundColor: '#ff7675',
    padding: 10
  },
  instructions: {
    textAlign: 'center',
    color: '#333333',
    marginBottom: 5,
  },
});
