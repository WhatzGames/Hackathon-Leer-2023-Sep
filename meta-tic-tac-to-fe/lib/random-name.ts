const names = ['Jar Jar Binks', 'Amy Schumer', 'UnityEngine', 'Moq', 'Elon Musk', 'Martin Shkreli', 'Andrew Wilson',
  'X Æ A-12', 'Phallicator', 'Nutnibbler'];

export function randomName() {
  const index = Math.round(Math.random() * names.length);
  return names[index];
}