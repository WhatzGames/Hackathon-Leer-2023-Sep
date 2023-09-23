import {LucideCircle, LucideX} from 'lucide-react';

export function Player({children}: React.PropsWithChildren) {
  return (
    children === 'X'
      ? <LucideX size={48} className={'text-cyan-300'}/>
      : <LucideCircle size={48} className={'text-purple-400'}/>
  );
}