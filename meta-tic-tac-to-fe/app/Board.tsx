import {useGameStore} from '@/app/game-store';
import {cn} from '@/lib/cn';
import Field from '@/app/Field';

type BoardProps = {
  boardNo: number;
  fields: (string | null)[];
}

export default function Board({boardNo, fields}: BoardProps) {
  const activeBoard = useGameStore(state => state.activeBoard);
  const finishedBoards = useGameStore(state => state.overview);
  const winner = finishedBoards[boardNo];

  return (
    <div className={cn(
      'relative transition-all',
      activeBoard !== boardNo ? (activeBoard === null && !winner ? 'scale-100' : 'scale-90') : 'scale-110'
    )}>
      {(winner || (activeBoard !== null && activeBoard !== boardNo)) &&
        <div className={cn(
          'text-6xl text-red-white absolute bg-slate-700/60 w-full h-full flex justify-center rounded-lg',
          'items-center shadow-xl'
        )}>
          <span>{winner}</span>
        </div>
      }
      <div className={cn(
        'grid grid-cols-3 grid-rows-3 gap-1',
        winner && 'border-red-950'
      )}>
        {fields.map((field, index) => <Field key={index} board={boardNo} field={index}>{field}</Field>)}
      </div>
    </div>
  );
}
