'use client';

import {useGameStore} from '@/app/game-store';
import {cn} from '@/lib/cn';

export default function Game() {
  const {game} = useGameStore();

  return (
    <div className={'grid grid-cols-3 grid-rows-3 gap-3'}>
      {game.map((board, index) => <Board key={index} boardNo={index} board={board}/>)}
    </div>
  );
}

type BoardProps = {
  boardNo: number;
  board: (string | null)[];
}

const Board = ({boardNo, board}: BoardProps) => {
  const activeBoard = useGameStore(state => state.activeBoard);
  const finishedBoards = useGameStore(state => state.finishedBoards);
  const winner = finishedBoards[boardNo];

  return (
    <div className={'relative'}>
      {winner &&
        <div className={'text-6xl text-red-500 absolute bg-slate-700/60 w-full h-full flex justify-center items-center'}>
          {winner}
        </div>
      }
      <div className={cn(
        'grid grid-cols-3 grid-rows-3 gap-1',
        activeBoard === boardNo && 'border-2 border-red-600',
        winner && 'border-red-950'
      )}>
        {board.map((field, index) => <Field key={index} board={boardNo} field={index}>{field}</Field>)}
      </div>
    </div>
  );
};

type FieldProps = {
  board: number;
  field: number;
  width?: number;
  height?: number;
}

const Field = ({board, field, width = 60, height = 60, children}: React.PropsWithChildren<FieldProps>) => {
  const setField = useGameStore(state => state.setField);

  const handleClick = () => setField(board, field);

  return (
    <button className={'border border-cyan-400 hover:bg-cyan-300 hover:animate-pulse'}
            onClick={handleClick}
            style={{width, height}}>
      {children}
    </button>
  );
};
