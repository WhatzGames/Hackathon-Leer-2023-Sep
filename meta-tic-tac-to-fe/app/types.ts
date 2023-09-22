type Game = Board[];

type Board = {
  id: string;
  fields: (PlayerSymbol | null)[];
};

type PlayerSymbol = 'X' | 'O';

type Player = {
  id: string;
  symbol: PlayerSymbol;
  score: number;
};

export type {
  Game,
  Board,
  Player,
  PlayerSymbol
}