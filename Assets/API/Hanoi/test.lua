function move_stack(size, s, e, m)
    if size == 1 then
        api.MoveRing(s, e)
    else
        move_stack(size-1, s, m, e)
        api.MoveRing(s, e)
        move_stack(size-1, m, e, s)
    end
end

move_stack(4, 0, 2, 1)
