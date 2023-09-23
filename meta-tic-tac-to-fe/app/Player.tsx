import {LucideCircle, LucideX} from 'lucide-react';
import {animated, useTransition} from '@react-spring/web';

export function Player({children}: React.PropsWithChildren) {
  const transitions = useTransition(true, {
    from: { opacity: 0 },
    enter: { opacity: 1, config: {duration: 100} },
  })

  return (
    transitions((style, item) => (
      item &&
      <animated.div style={style}>
        {children === 'X'
          ? <LucideX size={48} className={'text-cyan-300'}/>
          : <LucideCircle size={48} className={'text-purple-400'}/>}
      </animated.div>
    ))
  );
}