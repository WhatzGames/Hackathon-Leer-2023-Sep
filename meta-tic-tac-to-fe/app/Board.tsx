import {useGameStore} from '@/app/game-store';
import {cn} from '@/lib/cn';
import Field from '@/app/Field';

type BoardProps = {
  boardNo: number;
  fields: (string | null)[];
  view?: boolean;
}

export default function Board({boardNo, fields, view = false}: BoardProps) {
  const activeBoard = useGameStore(state => state.activeBoard);
  const finishedBoards = useGameStore(state => state.overview);
  const winner = finishedBoards[boardNo];

  return (
    <div className={cn(
      'relative transition-all',
      activeBoard !== boardNo ? (activeBoard === null && !winner ? 'scale-100' : 'scale-90') : 'scale-110'
    )}>
      {(winner || (activeBoard !== null && activeBoard !== boardNo)) &&
        <>
          <div className={cn(
            'text-6xl text-red-white absolute z-20 bg-slate-700/60 w-full h-full flex justify-center rounded-lg',
            'items-center shadow-xl'
          )}>
            {winner &&
              <div className={'flex items-center justify-center bg-white rounded-full'} style={{width: 100, height: 100}}>
                <div className={'h-fit'}>{winner}</div>
              </div>
            }
          </div>
        </>
      }
      <div className={cn(
        'grid grid-cols-3 grid-rows-3 gap-1',
        winner && 'border-red-950'
      )}>
        {fields.map((field, index) => (
          <Field key={index} board={boardNo} field={index} view={view}>{field}</Field>
        ))}
      </div>
    </div>
  );
}
