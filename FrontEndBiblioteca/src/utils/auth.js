// src/utils/auth.js
import Cookies from 'js-cookie';

export const authHeaders = () => {
  const token = Cookies.get('auth_token');
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`,
  };
};
