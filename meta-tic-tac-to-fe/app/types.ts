type Game = Board[];

type Board = {
  id: string;
  fields: (PlayerSymbol | null)[];
};

type PlayerId = string;
type PlayerSymbol = 'X' | 'O';

type Player = {
  id: PlayerId;
  symbol: PlayerSymbol;
  score: number;
};

type PlayedGame = {
  id: string;
  players: Player[];
  overview: Board;
  log: {
    player: string;
    move: [number, number];
  }[];
  type: 'RESULT';
  self: PlayerId;
}

export type {
  Game,
  Board,
  Player,
  PlayedGame,
  PlayerSymbol
}