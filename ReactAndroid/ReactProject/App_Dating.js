/**
 * Sample Dating React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React, {Component} from 'react';
import {Platform, StyleSheet, Text, View, Button, ScrollView, TextInput, TouchableOpacity} from 'react-native';

import { Avatar, Badge } from 'react-native-elements'

const list = [
  {
    name: 'Amy Farha',
    avatar_url: 'https://s3.amazonaws.com/uifaces/faces/twitter/ladylexy/128.jpg',
    subtitle: 'Weird Animals'
  },
  {
    name: 'Chris Jackson',
    avatar_url: 'https://s3.amazonaws.com/uifaces/faces/twitter/adhamdannaway/128.jpg',
    subtitle: 'Belly Flopping'
  },
  {
    name: 'Kristen Button',
    avatar_url: 'https://s3.amazonaws.com/uifaces/faces/twitter/kfriedson/128.jpg',
    subtitle: 'Sky Diving'
  },
  {
    name: 'Bryn Burton',
    avatar_url: 'https://s3.amazonaws.com/uifaces/faces/twitter/brynn/128.jpg',
    subtitle: 'Ice Creams'
  }
]

type Props = {};
export default class App extends Component<Props> {

  constructor(props) {
    super(props);
    this.state = { 
      isClick: true,
      title: "React-Native Dating",
      message: "(Use at your own risk!)",
      txtInput: "",
      index: 0
    };

    this.onPressButton = this.onPressButton.bind(this);
  }

  onPressButton(e) {
      let newIndex = this.state.index + 1;
      if (newIndex > 3) {
        newIndex = 0;
      }
      this.setState({
        index: newIndex
      });
  }

  render() {
    let urlImage = String(list[this.state.index].avatar_url);
    let imgUri = {
      uri: urlImage
    }
    let renderVar = 
    <ScrollView contentContainerStyle={styles.container}>
        <Text style={styles.welcome}>{this.state.title}</Text>
        <Text style={styles.instructions}>{this.state.message}</Text>
        <Avatar
          xlarge
          rounded
          source={imgUri}
          onPress={this.onPressButton}
          activeOpacity={0.7}
        />


        <Text style={styles.welcome}>{list[this.state.index].name}</Text>

        <Text style={styles.welcome}>Interests </Text>
        <Badge value={list[this.state.index].subtitle} textStyle={{ color: 'orange' }}/>

        <Text style={styles.welcome}>Tap to Like</Text>
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
