import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { Member } from '../models/member';
import { MembersService } from '../services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MembersService);

  return memberService.getMember(route.paramMap.get('username')!)
};
