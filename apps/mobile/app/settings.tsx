import { View, Text, ScrollView, TouchableOpacity } from 'react-native';
import { useT, useLocale } from './i18n';
import { useEffect, useState } from 'react';
import '../global.css';

const languages = [
  { code: 'en', label: 'English' },
  { code: 'fr', label: 'Français' },
  { code: 'de', label: 'Deutsch' },
  { code: 'nl', label: 'Nederlands' },
];

export default function Settings() {
  const { locale, setLocale } = useLocale();
  const [selected, setSelected] = useState<string>(locale);

  const t = useT();

  useEffect(() => {
    setSelected(locale);
  }, [locale]);

  const choose = async (code: string) => {
    await setLocale(code);
    setSelected(code);
  };

  return (
    <ScrollView className="flex-1 bg-gray-100">
      <View className="p-5">
        <Text className="text-2xl font-bold mb-4 text-gray-800">{t('settings.languageTitle')}</Text>
        {languages.map((lang) => (
          <TouchableOpacity
            key={lang.code}
            className={`p-4 rounded-lg mb-2 ${selected === lang.code ? 'bg-primary' : 'bg-white'}`}
            onPress={() => choose(lang.code)}
          >
            <Text className={`text-base ${selected === lang.code ? 'text-white' : 'text-gray-800'}`}>{lang.label}</Text>
          </TouchableOpacity>
        ))}
      </View>
    </ScrollView>
  );
}
