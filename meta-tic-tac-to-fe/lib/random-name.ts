const names = ['Jar Jar Binks', 'Amy Schumer', 'UnityEngine', 'Moq', 'Elon Musk', 'Martin Shkreli', 'Andrew Wilson',
  'X Æ A-12', 'Phallicator', 'Nutnibbler', 'Bowser', 'C64', 'N64', 'Sega Megadrive', 'Sega Dreamcast'];

export function randomName() {
  const index = Math.round(Math.random() * (names.length - 1));
  return names[index];
}