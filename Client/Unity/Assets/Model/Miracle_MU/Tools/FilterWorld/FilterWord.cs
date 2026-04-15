
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ETModel;
namespace ETModel
{
    /// <summary>
    /// 非法关键词过滤(自动忽略汉字数字字母间的其他字符)
    /// </summary>
    public class FilterWord
    {
        public FilterWord()
        {
            AssetBundleComponent.Instance.LoadBundle("dirtywords".StringToAB());
            TextAsset asset = (TextAsset)AssetBundleComponent.Instance.GetAsset("dirtywords".StringToAB(), "dirtywords");
           // ReferenceCollector monoReference = (AssetBundleComponent.Instance.GetAsset("dirtywords".StringToAB(), "dirtywords") as GameObject).GetReferenceCollector();
           // TextAsset asset = monoReference.GetTextAsset("dirtywords");
            m_AllFilterWord = asset.text;
            AssetBundleComponent.Instance.UnloadBundle("dirtywords".StringToAB());
        }

        private string m_AllFilterWord = string.Empty;
        /// <summary>
        /// 词库路径
        /// </summary>
        public string AllFilterWord
        {
            get { return m_AllFilterWord; }
            set { m_AllFilterWord = value; }
        }

        /// <summary>
        /// 内存词典
        /// </summary>
        private WordGroup[] MEMORYLEXICON = new WordGroup[(int)char.MaxValue];

        private string sourctText = string.Empty;
        private bool m_IsInitalize = false;
        /// <summary>
        /// 检测源
        /// </summary>
        public string SourceText
        {
            get { return sourctText; }
            set { sourctText = value; }
        }

        /// <summary>
        /// 检测源游标
        /// </summary>
        int cursor = 0;

        /// <summary>
        /// 匹配成功后偏移量
        /// </summary>
        int wordlenght = 0;

        /// <summary>
        /// 检测词游标
        /// </summary>
        int nextCursor = 0;

        private List<string> illegalWords = new List<string>();

        /// <summary>
        /// 检测到的非法词集
        /// </summary>
        public List<string> IllegalWords
        {
            get { return illegalWords; }
        }

        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool isCHS(char character)
        {
            //  中文表意字符的范围 4E00-9FA5
            int charVal = (int)character;
            return (charVal >= 0x4e00 && charVal <= 0x9fa5);
        }

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool isNum(char character)
        {
            int charVal = (int)character;
            return (charVal >= 48 && charVal <= 57);
        }

        /// <summary>
        /// 判断是否是字母
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool isAlphabet(char character)
        {
            int charVal = (int)character;
            return ((charVal >= 97 && charVal <= 122) || (charVal >= 65 && charVal <= 90));
        }

        /// <summary>
        /// 转半角小写的函数(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        private string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c).ToLower();
        }

        /// <summary>
        /// 加载内存词库
        /// </summary>
        public void LoadDictionary()
        {
            if (m_IsInitalize)
            {
                return;
            }

            m_IsInitalize = true;
            List<string> wordList = new List<string>();
            Array.Clear(MEMORYLEXICON, 0, MEMORYLEXICON.Length);
            string[] words = AllFilterWord.Split('\n');
            foreach (string word in words)
            {
                string str = word.Replace("\r", "");
                string key = this.ToDBC(str);
                wordList.Add(key);
            }
            Comparison<string> cmp = delegate (string key1, string key2)
            {
                return key1.CompareTo(key2);
            };
            wordList.Sort(cmp);
            for (int i = wordList.Count - 1; i > 0; i--)
            {
                if (wordList[i].ToString() == wordList[i - 1].ToString())
                {
                    wordList.RemoveAt(i);
                }
            }
            foreach (var word in wordList)
            {
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                WordGroup group = MEMORYLEXICON[word[0]];
                if (group == null)
                {
                    group = new WordGroup();
                    MEMORYLEXICON[(int)word[0]] = group;

                }
                group.Add(word.Substring(1));
            }
        }

        /// <summary>
        /// 检测
        /// </summary>
        /// <param name="blackWord"></param>
        /// <returns></returns>
        private bool Check(string blackWord)
        {
            wordlenght = 0;
            //检测源下一位游标
            nextCursor = cursor + 1;
            bool found = false;
            string tempStr = ToDBC(sourctText);
            //遍历词的每一位做匹配
            for (int i = 0; i < blackWord.Length; i++)
            {
                //特殊字符偏移游标
                int offset = 0;
                if (nextCursor >= tempStr.Length)
                {
                    break;
                }
                else
                {
                    if (i >= blackWord.Length
                        || nextCursor + offset >= tempStr.Length)
                    {
                        found = false;
                        break;
                    }
                    if ((int)blackWord[i] == (int)tempStr[nextCursor + offset])
                    {
                        if (isAlphabet(tempStr[nextCursor + offset]))
                        {
                            if (tempStr.Length < blackWord.Length)
                            {
                                found = false;
                                break;
                            }
                            if (i >= blackWord.Length - 1)
                            {
                                int temp = nextCursor + offset + 1;
                                if (tempStr.Length > temp)
                                {
                                    if (isAlphabet(tempStr[temp]))
                                    {
                                        found = false;
                                        break;
                                    }
                                    else
                                    {
                                        found = true;
                                    }
                                }
                                else
                                {
                                    found = true;
                                }
                            }
                        }
                        else
                        {
                            if (i >= blackWord.Length - 1)
                            {
                                found = true;
                            }
                        }
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }

                nextCursor = nextCursor + 1 + offset;
                wordlenght++;
            }
            return found;
        }

        /// <summary>
        /// 查找并替换
        /// </summary>
        /// <param name="replaceChar"></param>
        public string Filter(char replaceChar)
        {
            cursor = 0;
            nextCursor = 0;
            LoadDictionary();
            if (sourctText != string.Empty)
            {
                //sourctText = sourctText.Replace("\n", "");
                //sourctText = sourctText.Trim();
                char[] tempString = sourctText.ToCharArray();
                for (int i = 0; i < SourceText.Length; i++)
                {
                    //查询以该字为首字符的词组
                    WordGroup group = MEMORYLEXICON[(int)ToDBC(SourceText)[i]];
                    if (group != null)
                    {
                        for (int z = 0; z < group.Count(); z++)
                        {
                            string word = group.GetWord(z);
                            if (word.Length == 0 || Check(word))
                            {
                                string blackword = string.Empty;
                                for (int pos = 0; pos < wordlenght + 1; pos++)
                                {
                                    blackword += tempString[pos + cursor].ToString();
                                    tempString[pos + cursor] = replaceChar;
                                }
                                illegalWords.Add(blackword);
                                cursor = cursor + wordlenght;
                                i = i + wordlenght;
                            }
                        }
                    }
                    cursor++;
                }
                return new string(tempString);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 具有相同首字符的词组集合
    /// </summary>
    class WordGroup
    {
        /// <summary>
        /// 集合
        /// </summary>
        private List<string> groupList;

        public WordGroup()
        {
            groupList = new List<string>();
        }

        /// <summary>
        /// 添加词
        /// </summary>
        /// <param name="word"></param>
        public void Add(string word)
        {
            groupList.Add(word);
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return groupList.Count;
        }

        /// <summary>
        /// 根据下标获取词
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWord(int index)
        {
            return groupList[index];
        }
    }
}