import React, { createContext, useContext, useEffect, useState } from 'react';
import translations from './translations.json';
import AsyncStorage from '@react-native-async-storage/async-storage';

const STORAGE_KEY = 'agility_scoring_locale';

let locale = 'en';

export async function initLocale() {
  try {
    const stored = await AsyncStorage.getItem(STORAGE_KEY);
    if (stored) locale = stored;
  } catch (e) {
    // ignore
  }
}

// persist locale to storage and update module-level value
export async function setLocale(l: string) {
  locale = l;
  try {
    await AsyncStorage.setItem(STORAGE_KEY, l);
  } catch (e) {
    // ignore
  }
}

export function t(key: string) {
  const entry = (translations as any)[key];
  if (!entry) return key;
  return entry[locale] ?? entry['en'] ?? Object.values(entry)[0] ?? key;
}

export function currentLocale() {
  return locale;
}

type LocaleContextShape = {
  locale: string;
  setLocale: (l: string) => Promise<void>;
};

export const LocaleContext = createContext<LocaleContextShape>({
  locale: 'en',
  setLocale: async () => {},
});

export function LocaleProvider({ children }: { children: React.ReactNode }) {
  const [loc, setLoc] = useState<string>(locale);

  useEffect(() => {
    // initialize from storage
    (async () => {
      await initLocale();
      setLoc(locale);
    })();
  }, []);

  const setLocaleAndState = async (l: string) => {
    await setLocale(l);
    setLoc(l);
  };

  return (
    <LocaleContext.Provider value={{ locale: loc, setLocale: setLocaleAndState }}>
      {children}
    </LocaleContext.Provider>
  );
}

export function useLocale() {
  return useContext(LocaleContext);
}

export function useT() {
  const { locale } = useLocale();
  return (key: string) => {
    const entry = (translations as any)[key];
    if (!entry) return key;
    return entry[locale] ?? entry['en'] ?? Object.values(entry)[0] ?? key;
  };
}
